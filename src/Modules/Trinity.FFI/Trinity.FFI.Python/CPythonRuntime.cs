﻿using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Storage;
using Python.Runtime;
using Trinity.Utilities;
using System.IO;
using Trinity.Extension;
using Trinity.Diagnostics;
using System.Reflection;

namespace Trinity.FFI.Python
{
    class CPythonRuntime : ILanguageRuntime
    {
        private Py.GILState m_GIL;
        private PyScope m_scope;

        public CPythonRuntime()
        {
            PythonEngine.PythonPath += ";" + Path.GetDirectoryName(typeof(CPythonRuntime).Assembly.Location);
            Log.WriteLine("PythonPath = {0}", PythonEngine.PythonPath);
            m_GIL = Py.GIL();
            m_scope = Py.CreateScope();
            //TODO make sure the GraphEngine wheel is installed
            m_scope.Exec(@"import GraphEngine");
        }

        public void Dispose()
        {
            m_GIL.Dispose();
            PythonEngine.Shutdown();
        }

        public int LoadProgram(string path)
        {
            string content = File.ReadAllText(path);
            m_scope.Exec(content);
            return 0;
        }

        public string SynHandler(int methodId, string input)
        {
            return "";
        }

        public void AsynHandler(int methodId, string input)
        {
            return;
        }
    }
}
