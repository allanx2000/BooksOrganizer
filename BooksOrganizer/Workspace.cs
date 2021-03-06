﻿using BooksOrganizer.Models;
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
            this.Directory = (new FileInfo(path)).DirectoryName;

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
        public string Directory { get; private set; }

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

        //TODO: Pre-Compile?
        public ICollection<Topic> GetAllTopics()
        {
            return Workspace.Current.DB.Topics.OrderBy(x => x.Name).ToList();
        }

        /*
        public ICollection<SubTopic> GetAllSubTopics(Topic selectedTopic)
        {
            return (from st in Workspace.Current.DB.SubTopics
                   where st.ParentTopicId == selectedTopic.ID
                   orderby st.Name ascending
                   select st).ToList();
        }*/

        public ICollection<Book> GetAllBooks()
        {
            return (from bk in Workspace.Current.DB.Books
                    orderby bk.Title ascending
                    select bk).ToList();
        }

        public Dictionary<int, List<Note>> GetAllNotesGrouped(bool unpublishedOnly = false)
        {
            Dictionary<int, List<Note>> lookup = new Dictionary<int, List<Note>>(); //Book, List

            IEnumerable<Note> query;

            if (unpublishedOnly)
                query = query = (from n in Current.DB.Notes
                                 where n.Published == false
                                 orderby n.Location ascending //TODO: Make int? 
                                 select n);
            else
                query = query = (from n in Current.DB.Notes
                                 orderby n.Location ascending
                                 select n);
            
            foreach (Note n in query)
            {
                if (!lookup.ContainsKey(n.BookId))
                    lookup.Add(n.BookId, new List<Note>());

                lookup[n.BookId].Add(n);
            }

            return lookup;
        }

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
