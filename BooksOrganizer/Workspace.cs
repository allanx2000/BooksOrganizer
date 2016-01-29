using BooksOrganizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IOP = System.IO.Path;

namespace BooksOrganizer
{
    public interface IWorkspace //Class Model
    {
        string Path {get; }
        void Close(bool throwError = false);
    }

    /// <summary>
    /// Repository/Service that represents the loaded workspace
    /// </summary>
    public class Workspace : IWorkspace
    {
        #region Static

        /// <summary>
        /// The current loaded workspace
        /// If null, no workspace loaded
        /// </summary>
        public static Workspace Current { get; private set; }

        /// <summary>
        /// Loads the workspace from a folder. The structure of the workspace must be consistent.
        /// </summary>
        /// <param name="path">Folder containing the workspace</param>
        public static void LoadWorkspace(string path)
        {
            Current = new Workspace(path, true);

            var i = Current.DB.Notes.FirstOrDefault();
        }

        /// <summary>
        /// Creates a new workspace folder
        /// </summary>
        /// <param name="path">Path to directory to create the workspace in (must not exist)</param>
        /// <param name="name">Name of the company</param>
        /// <param name="ticker"></param>
        public static void CreateWorkspace(string path)
        {
            Current = new Workspace(path, false);
        }

        #endregion

        private readonly WorkspaceContext db;

        internal WorkspaceContext DB
        {
            get
            {
                return db;
            }
        }

        #region Private

        private const string DbFile = "db.sqlite";

        private Workspace(string path, bool exists)
        {
            if (!exists)
            {
                CreateDB(path);
            }
            else
            {
                FileInfo fi = new FileInfo(path);

                if (!fi.Exists)
                    throw new Exception("File not found: " + path);
            }

            this.Path = path;

            db = WorkspaceContext.GetContext(path);
        }

        private void CreateDB(string path)
        {
            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
                throw new Exception("The database already exists.");

            File.Copy(DbFile, path);
        }

        #endregion


        /// <summary>
        /// Workspace Path (Directory)
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        /// <summary>
        /// Does a final save of the workspace and closes it
        /// </summary>
        public void Close(bool throwError = false)
        {
            try
            {
                db.SaveChanges();

                Current.DB.Database.Connection.Close();

                Current = null;
            }
            catch (Exception e)
            {
                if (throwError)
                    throw;
            }
        }

        /*
        #region Documents

        public void OpenDocument(Document document)
        {
            var path = IOP.Combine(GetDocumentsFolder(), document.FileLocation);
            Process.Start(path);
        }

        /// <summary>
        /// Returns the full path to the Documents folder inside the Workspace
        /// </summary>
        /// <returns></returns>
        public string GetDocumentsFolder()
        {
            return IOP.Combine(Path, DocumentsFolder);
        }

        public void AddDocument(Document doc)
        {
            bool copied = false;
            string path = null;

            try
            {
                var document = FindDocument(doc.DocumentDate, doc.Type);

                if (document != null)
                    throw new Exception("Document already exists");

                //Validate File
                if (String.IsNullOrEmpty(doc.FileLocation))
                    throw new Exception("FileLocation is empty");

                var file = new FileInfo(doc.FileLocation);

                if (!file.Exists)
                    throw new Exception("File does not exist: " + file);

                //Copy File
                string name = CreateFileName(doc.DocumentDate, doc.Type, file.Extension);

                path = IOP.Combine(GetDocumentsFolder(), name);

                file.CopyTo(path);

                copied = true;

                doc.FileLocation = name;

                //Save To DB
                DB.Documents.Add(doc);

                DB.SaveChanges();

                NeedsReload = true;
            }
            catch (Exception)
            {
                if (copied)
                    File.Delete(path);

                throw;
            }
        }

        //Note: EF can only deals with primitive types x.Type == documentType will not work

        /// <summary>
        /// Finds a document of the given type and date
        /// </summary>
        /// <param name="date"></param>
        /// <param name="documentType"></param>
        /// <returns>The Document or null</returns>
        public Document FindDocument(DateTime date, DocumentType documentType)
        {
            if (NeedsReload)
            {
                var ws = Reload();

                return ws.FindDocument(date, documentType);
            }

            var data = db.Documents.Where(x => x.DocumentTypeID == documentType.ID
                && x.DocumentDate == date);

            return data.FirstOrDefault();
        }

        /// <summary>
        /// Files are stored in a special format using the date and type
        /// </summary>
        /// <param name="date"></param>
        /// <param name="type"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        private string CreateFileName(DateTime date, DocumentType type, string ext)
        {
            string datePart;

            switch (type.DateType)
            {
                case DocumentType.DateTypeEnum.Annual:
                    datePart = date.Year.ToString();
                    break;
                case DocumentType.DateTypeEnum.Exact:
                    datePart = date.ToString("yyyy_MM_dd");
                    break;
                case DocumentType.DateTypeEnum.Quarterly:
                    datePart = date.ToString("yyyy_MM");
                    break;
                default:
                    throw new NotRecognizedException(type.DateType);
            }

            string name = String.Join("_", datePart, type.Code) + ext;
            return name;
        }

        /// <summary>
        /// Gets the documents for a given year and type
        /// </summary>
        /// <param name="year"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Document> GetDocuments(int? year, DocumentType type)
        {

            if (NeedsReload)
            {
                var ws = Reload();

                return ws.GetDocuments(year, type);
            }

            var query = db.Documents.AsQueryable();

            if (year != null)
                query = query.Where(x => x.DocumentDate.Year == year);

            if (type != null)
                query = query.Where(x => x.DocumentTypeID == type.ID);

            return query;
        }

        public void DeleteDocument(Document document)
        {
            string file = document.FileLocation;

            DB.Documents.Remove(document);
            db.SaveChanges();


            File.Delete(IOP.Combine(GetDocumentsFolder(), file));
        }


        public Document GetDocument(int id)
        {

            if (NeedsReload)
            {
                var ws = Reload();

                return ws.GetDocument(id);
            }


            return DB.Documents.Where(x => x.ID == id).First();
        }

        #endregion

        #region DocumentType

        /// <summary>
        /// Returns all DocumentTypes registered to the Workspace
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DocumentType> GetDocumentTypes()
        {
            var types = from d in db.DocumentTypes
                        select d;

            return types;
        }

        public DocumentType GetDocumentType(string code)
        {
            code = code.ToUpper();

            var result = from d in db.DocumentTypes
                         where d.Code == code
                         select d;

            if (result.Count() == 0)
                return null;
            else
                return result.First();
        }

        public void AddDocumentType(DocumentType type)
        {
            type.Code = type.Code.ToUpper();

            ValidateDocumentType(type);

            var exists = GetDocumentType(type.Code);
            if (exists != null)
                throw new Exception("The type " + type.Code + " already exists.");

            db.DocumentTypes.Add(type);
            db.SaveChanges();
        }

        private void ValidateDocumentType(DocumentType type)
        {
            if (String.IsNullOrEmpty(type.Name.Trim()))
                throw new Exception("Name cannot be empty.");

            if (!alpha.IsMatch(type.Code))
                throw new Exception("Code must be alphanumeric only.");
        }

        public void UpdateDocumentType(DocumentType updated)
        {
            ValidateDocumentType(updated);
            db.SaveChanges();

        }

        #endregion

        #region Facts
        #region FactObject

        /// <summary>
        /// Returns all the FactDefinitions and Groups in the Workspace
        /// </summary>
        /// <returns></returns>
        public List<FactObject> GetAllFactObjects()
        {
            List<FactObject> results = new List<FactObject>();

            results.AddRange(GetAllFactsGroups());
            results.AddRange(GetAllFactDefinitions());

            return results;
        }

        /// <summary>
        /// Returns all children FactDefinitions and Groups in a list (not tree)
        /// 
        /// It does not include the children of children Groups
        /// </summary>
        /// <param name="parentGroupId"></param>
        /// <returns></returns>
        public List<FactObject> GetFactObjects(int? parentGroupId)
        {
            if (NeedsReload)
            {
                Reload();

                return Workspace.Current.GetFactObjects(parentGroupId);
            }

            List<FactObject> results = new List<FactObject>();

            var groups = DB.FactsGroups.AsQueryable();

            groups = groups.Where(x => x.ParentGroupId == parentGroupId);
            groups = groups.OrderBy(x => x.Name);

            var list = groups.ToList();
            results.AddRange(list);

            var factTypes = DB.FactDefinitions.AsQueryable();

            factTypes = factTypes.Where(x => x.ParentGroupId == parentGroupId);
            factTypes = factTypes.OrderBy(x => x.Name);

            results.AddRange(factTypes.ToList());

            return results;
        }


        #endregion

        #region FactValue

        /// <summary>
        /// Fact needs to be added manually by calling function to the document itself
        /// </summary>
        /// <param name="fact"></param>
        public void AddFactValue(FactValue fact)
        {
            //TODO: All validation must be on model
            ValidateFact(fact);

            DB.FactValues.Add(fact);
            DB.SaveChanges();

            NeedsReload = true;
        }

        public void RemoveFactValue(FactValue fact)
        {
            var doc = fact.Document;

            DB.FactValues.Remove(fact);
            DB.SaveChanges();

            doc.FactValues.Remove(fact);
        }

        public void UpdateFactValue(FactValue fact)
        {
            //TODO: Validate

            DB.SaveChanges();
        }

        private void ValidateFact(FactValue fact)
        {
            //throw new NotImplementedException();
        }

        #endregion



        #region FactsGroup
        public List<FactsGroup> GetAllFactsGroups()
        {
            return db.FactsGroups.OrderBy(x => x.Name).ToList();
        }


        public void DeleteFactsGroup(FactsGroup group)
        {
            //Null all dependencies
            FactsGroup parentGroup = group.ParentGroup;

            var children = DB.FactDefinitions.Where(x => x.ParentGroupId == group.ID);
            if (children.Count() > 0)
            {
                foreach (var c in children)
                {
                    c.ParentGroup = null;
                }
            }

            DB.FactsGroups.Remove(group);
            db.SaveChanges();
        }

        public FactsGroup GetFactsGroup(int id)
        {
            if (NeedsReload)
            {
                Reload();

                return Workspace.Current.GetFactsGroup(id);
            }

            return DB.FactsGroups.FirstOrDefault(x => x.ID == id);
        }

        public List<FactsGroup> GetFactsGroups(int? parentId)
        {
            var query = DB.FactsGroups
                .Where(x => x.ParentGroupId == parentId)
                .OrderBy(x => x.Name);

            return query.ToList();
        }

        //Only really used for testing...
        public bool HasFactGroups()
        {
            return DB.FactsGroups.Count() > 0;
        }

        public void UpdateFactsGroup(FactsGroup g)
        {
            ValidateFactsGroup(g);
            db.SaveChanges();

            NeedsReload = true;
        }

        private void ValidateFactsGroup(FactsGroup g)
        {
            //TODO: Add validation
        }

        public void AddFactsGroup(FactsGroup g)
        {
            ValidateFactsGroup(g);

            DB.FactsGroups.Add(g);
            DB.SaveChanges();

            NeedsReload = true;
        }

        #endregion

        #region FactDefinitions
        public List<FactDefinition> GetAllFactDefinitions()
        {
            if (NeedsReload)
            {
                var ws = Reload();

                return ws.GetAllFactDefinitions();
            }

            return db.FactDefinitions.OrderBy(x => x.Name).ToList();
        }


        public void UpdateFactDefinition(FactDefinition def)
        {
            //Validate?
            DB.SaveChanges();
        }

        /// <summary>
        /// Reject all changes made to the models since last save (Revert)
        /// </summary>
        private void RejectChanges()
        {
            foreach (var entry in DB.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }

        public void AddFactDefinition(FactDefinition d)
        {
            try
            {
                DB.FactDefinitions.Add(d);
                DB.SaveChanges();


                NeedsReload = true;
            }
            catch (Exception e)
            {
                RejectChanges();
                throw;
            }
        }

        public void DeleteFactDefinition(FactDefinition def)
        {
            DB.FactDefinitions.Remove(def);
            DB.SaveChanges();
        }


        #endregion
        #endregion
        
        */

        #region Reloads

        /*
        public Workspace Reload(bool failSilently = true)
        {
            try
            {

                if (AutoReloadEnabled)
                    DB.Database.Initialize(true);

                NeedsReload = false;

                return Workspace.Current;
            }
            catch (Exception e)
            {
                throw;
            }
        }*/

        #endregion

    }
}
