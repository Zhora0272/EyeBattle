using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Vengadores.Utility.EmptyFolderRemover.Editor
{
    public class Core : UnityEditor.AssetModificationProcessor
    {
        public static void DeleteAllEmptyDirAndMeta( ref List<DirectoryInfo> emptyDirs )
        {
            foreach (var dirInfo in emptyDirs)
            {
                AssetDatabase.MoveAssetToTrash( GetRelativePathFromCd( dirInfo.FullName ) );
            }

            emptyDirs = null;
        }

        public static void FillEmptyDirList( out List<DirectoryInfo> emptyDirs )
        {
            var newEmptyDirs = new List<DirectoryInfo>();
            emptyDirs = newEmptyDirs;

            var assetDir = new DirectoryInfo(Application.dataPath);

            WalkDirectoryTree(assetDir, ( dirInfo, areSubDirsEmpty ) =>
            {
                var isDirEmpty = areSubDirsEmpty && DirHasNoFile (dirInfo);
                if ( isDirEmpty )
                    newEmptyDirs.Add(dirInfo);
                return isDirEmpty;
            });
        }

        // return: Is this directory empty?
        private delegate bool IsEmptyDirectory( DirectoryInfo dirInfo, bool areSubDirsEmpty );

        // return: Is this directory empty?
        private static bool WalkDirectoryTree(DirectoryInfo root, IsEmptyDirectory pred)
        {
            var subDirs = root.GetDirectories();

            var areSubDirsEmpty = true;
            foreach (var dirInfo in subDirs)
            {
                if ( false == WalkDirectoryTree(dirInfo, pred) )
                    areSubDirsEmpty = false;
            }

            var isRootEmpty = pred(root, areSubDirsEmpty);
            return isRootEmpty;
        }

        private static bool DirHasNoFile(DirectoryInfo dirInfo)
        {
            FileInfo[] files = null;

            try
            {
                files = dirInfo.GetFiles("*.*");
                files = files.Where ( x => ! IsMetaFile(x.Name) && ! IsSystemFile(x.Name)).ToArray ();
            }
            catch (Exception)
            {
                // ignored
            }

            return files == null || files.Length == 0;
        }

        private static string GetRelativePathFromCd(string filespec)
        {
            return GetRelativePath( filespec, Directory.GetCurrentDirectory() );
        }

        public static string GetRelativePath(string filespec, string folder)
        {
            var pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            var folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        private static bool IsMetaFile(string path)
        {
            return path.EndsWith(".meta");
        }

        private static bool IsSystemFile(string path)
        {
            return path.StartsWith(".");
        }
    }
}