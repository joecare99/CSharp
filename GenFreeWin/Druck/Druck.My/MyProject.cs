using Druck.Views;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Druck.My
{
    [HideModuleName]
    [GeneratedCode("MyTemplate", "8.0.0.0")]
    [StandardModule]
    internal sealed class MyProject
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
        internal sealed class MyForms
        {
            public Ahnen m_Ahnen;

            public AhneneST m_AhneneST;

            public Ahnengem m_Ahnengem;

            public AhnenST m_AhnenST;

            public Ahnent m_Ahnent;

            public Anzeige m_Anzeige;

            public AT6 m_AT6;

            public ATMenue m_ATMenue;

            public ATo m_ATo;

            public Ausw m_Ausw;

            public AW m_AW;

            public Druck.Views.Druck m_Druck;

            public FaBu m_FaBu;

            public Familie m_Familie;

            public Generatio m_Generatio;

            public Hinter m_Hinter;

            public Nachlist m_Nachlist;

            public Namensuch m_Namensuch;

            public Ort m_Ort;

            public ortw m_ortw;

            public Personen m_Personen;

            public Sippenlist m_Sippenlist;

            public Stammfolge m_Stammfolge;

            [ThreadStatic]
            private static Hashtable m_FormBeingCreated;

            public Ahnen Ahnen
            {
                get
                {
                    m_Ahnen = Create__Instance__(m_Ahnen);
                    return m_Ahnen;
                }
                set
                {
                    if (value != m_Ahnen)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Ahnen);
                    }
                }
            }

            public AhneneST AhneneST
            {
                get
                {
                    m_AhneneST = Create__Instance__(m_AhneneST);
                    return m_AhneneST;
                }
                set
                {
                    if (value != m_AhneneST)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_AhneneST);
                    }
                }
            }

            public Ahnengem Ahnengem
            {
                get
                {
                    m_Ahnengem = Create__Instance__(m_Ahnengem);
                    return m_Ahnengem;
                }
                set
                {
                    if (value != m_Ahnengem)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Ahnengem);
                    }
                }
            }

            public AhnenST AhnenST
            {
                get
                {
                    m_AhnenST = Create__Instance__(m_AhnenST);
                    return m_AhnenST;
                }
                set
                {
                    if (value != m_AhnenST)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_AhnenST);
                    }
                }
            }

            public Ahnent Ahnent
            {
                get
                {
                    m_Ahnent = Create__Instance__(m_Ahnent);
                    return m_Ahnent;
                }
                set
                {
                    if (value != m_Ahnent)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Ahnent);
                    }
                }
            }

            public Anzeige Anzeige
            {
                get
                {
                    m_Anzeige = Create__Instance__(m_Anzeige);
                    return m_Anzeige;
                }
                set
                {
                    if (value != m_Anzeige)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Anzeige);
                    }
                }
            }

            public AT6 AT6
            {
                get
                {
                    m_AT6 = Create__Instance__(m_AT6);
                    return m_AT6;
                }
                set
                {
                    if (value != m_AT6)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_AT6);
                    }
                }
            }

            public ATMenue ATMenue
            {
                get
                {
                    m_ATMenue = Create__Instance__(m_ATMenue);
                    return m_ATMenue;
                }
                set
                {
                    if (value != m_ATMenue)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_ATMenue);
                    }
                }
            }

            public ATo ATo
            {
                get
                {
                    m_ATo = Create__Instance__(m_ATo);
                    return m_ATo;
                }
                set
                {
                    if (value != m_ATo)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_ATo);
                    }
                }
            }

            public Ausw Ausw
            {
                get
                {
                    m_Ausw = Create__Instance__(m_Ausw);
                    return m_Ausw;
                }
                set
                {
                    if (value != m_Ausw)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Ausw);
                    }
                }
            }

            public AW AW
            {
                get
                {
                    m_AW = Create__Instance__(m_AW);
                    return m_AW;
                }
                set
                {
                    if (value != m_AW)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_AW);
                    }
                }
            }

            public Druck.Views.Druck Druck
            {
                get
                {
                    m_Druck = Create__Instance__(m_Druck);
                    return m_Druck;
                }
                set
                {
                    if (value != m_Druck)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Druck);
                    }
                }
            }

            public FaBu FaBu
            {
                get
                {
                    m_FaBu = Create__Instance__(m_FaBu);
                    return m_FaBu;
                }
                set
                {
                    if (value != m_FaBu)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_FaBu);
                    }
                }
            }

            public Familie Familie
            {
                get
                {
                    m_Familie = Create__Instance__(m_Familie);
                    return m_Familie;
                }
                set
                {
                    if (value != m_Familie)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Familie);
                    }
                }
            }

            public Generatio Generatio
            {
                get
                {
                    m_Generatio = Create__Instance__(m_Generatio);
                    return m_Generatio;
                }
                set
                {
                    if (value != m_Generatio)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Generatio);
                    }
                }
            }

            public Hinter Hinter
            {
                get
                {
                    m_Hinter = Create__Instance__(m_Hinter);
                    return m_Hinter;
                }
                set
                {
                    if (value != m_Hinter)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Hinter);
                    }
                }
            }

            public Nachlist Nachlist
            {
                get
                {
                    m_Nachlist = Create__Instance__(m_Nachlist);
                    return m_Nachlist;
                }
                set
                {
                    if (value != m_Nachlist)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Nachlist);
                    }
                }
            }

            public Namensuch Namensuch
            {
                get
                {
                    m_Namensuch = Create__Instance__(m_Namensuch);
                    return m_Namensuch;
                }
                set
                {
                    if (value != m_Namensuch)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Namensuch);
                    }
                }
            }

            public Ort Ort
            {
                get
                {
                    m_Ort = Create__Instance__(m_Ort);
                    return m_Ort;
                }
                set
                {
                    if (value != m_Ort)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Ort);
                    }
                }
            }

            public ortw ortw
            {
                get
                {
                    m_ortw = Create__Instance__(m_ortw);
                    return m_ortw;
                }
                set
                {
                    if (value != m_ortw)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_ortw);
                    }
                }
            }

            public Personen Personen
            {
                get
                {
                    m_Personen = Create__Instance__(m_Personen);
                    return m_Personen;
                }
                set
                {
                    if (value != m_Personen)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Personen);
                    }
                }
            }

            public Sippenlist Sippenlist
            {
                get
                {
                    m_Sippenlist = Create__Instance__(m_Sippenlist);
                    return m_Sippenlist;
                }
                set
                {
                    if (value != m_Sippenlist)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Sippenlist);
                    }
                }
            }

            public Stammfolge Stammfolge
            {
                get
                {
                    m_Stammfolge = Create__Instance__(m_Stammfolge);
                    return m_Stammfolge;
                }
                set
                {
                    if (value != m_Stammfolge)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        Dispose__Instance__(ref m_Stammfolge);
                    }
                }
            }

            [DebuggerHidden]
            private static T Create__Instance__<T>(T Instance) where T : Form, new()
            {
                //Discarded unreachable code: IL_0076, IL_00cb, IL_00e7
                if (Instance == null || Instance.IsDisposed)
                {
                    if (m_FormBeingCreated != null)
                    {
                        if (m_FormBeingCreated.ContainsKey(typeof(T)))
                        {
                            throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate"));
                        }
                    }
                    else
                    {
                        m_FormBeingCreated = new Hashtable();
                    }
                    m_FormBeingCreated.Add(typeof(T), null);
                    try
                    {
                        return new T();
                    }
                    catch (TargetInvocationException ex) when (((Func<bool>)delegate
                    {
                        // Could not convert BlockContainer to single expression
                        ProjectData.SetProjectError(ex);
                        return ex.InnerException != null;
                    }).Invoke())
                    {
                        string resourceString = Utils.GetResourceString("WinForms_SeeInnerException", ex.InnerException.Message);
                        throw new InvalidOperationException(resourceString, ex.InnerException);
                    }
                    finally
                    {
                        m_FormBeingCreated.Remove(typeof(T));
                    }
                }
                return Instance;
            }

            [DebuggerHidden]
            private void Dispose__Instance__<T>(ref T instance) where T : Form
            {
                instance.Dispose();
                instance = null;
            }

            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public MyForms()
            {
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public override bool Equals(object o)
            {
                return base.Equals(o);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            internal new Type GetType()
            {
                return typeof(MyForms);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public override string ToString()
            {
                return base.ToString();
            }
        }

        [MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal sealed class MyWebServices
        {
            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override bool Equals(object o)
            {
                return base.Equals(o);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            [DebuggerHidden]
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            internal new Type GetType()
            {
                return typeof(MyWebServices);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            [DebuggerHidden]
            public override string ToString()
            {
                return base.ToString();
            }

            [DebuggerHidden]
            private static T Create__Instance__<T>(T instance) where T : new()
            {
                //Discarded unreachable code: IL_0010
                if (instance == null)
                {
                    return new T();
                }
                return instance;
            }

            [DebuggerHidden]
            private void Dispose__Instance__<T>(ref T instance)
            {
                instance = default(T);
            }

            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public MyWebServices()
            {
            }
        }

        [ComVisible(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal sealed class ThreadSafeObjectProvider<T> where T : new()
        {
            [CompilerGenerated]
            [ThreadStatic]
            private static T m_ThreadStaticValue;

            internal T GetInstance
            {
                [DebuggerHidden]
                get
                {
                    if (m_ThreadStaticValue == null)
                    {
                        m_ThreadStaticValue = new T();
                    }
                    return m_ThreadStaticValue;
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            [DebuggerHidden]
            public ThreadSafeObjectProvider()
            {
            }
        }

        private static readonly ThreadSafeObjectProvider<MyComputer> m_ComputerObjectProvider = new ThreadSafeObjectProvider<MyComputer>();

        private static readonly ThreadSafeObjectProvider<MyApplication> m_AppObjectProvider = new ThreadSafeObjectProvider<MyApplication>();

        private static readonly ThreadSafeObjectProvider<User> m_UserObjectProvider = new ThreadSafeObjectProvider<User>();

        private static ThreadSafeObjectProvider<MyForms> m_MyFormsObjectProvider = new ThreadSafeObjectProvider<MyForms>();

        private static readonly ThreadSafeObjectProvider<MyWebServices> m_MyWebServicesObjectProvider = new ThreadSafeObjectProvider<MyWebServices>();

        [HelpKeyword("My.Computer")]
        internal static MyComputer Computer
        {
            [DebuggerHidden]
            get
            {
                return m_ComputerObjectProvider.GetInstance;
            }
        }

        [HelpKeyword("My.Application")]
        internal static MyApplication Application
        {
            [DebuggerHidden]
            get
            {
                return m_AppObjectProvider.GetInstance;
            }
        }

        [HelpKeyword("My.User")]
        internal static User User
        {
            [DebuggerHidden]
            get
            {
                return m_UserObjectProvider.GetInstance;
            }
        }

        [HelpKeyword("My.Forms")]
        internal static MyForms Forms
        {
            [DebuggerHidden]
            get
            {
                return m_MyFormsObjectProvider.GetInstance;
            }
        }

        [HelpKeyword("My.WebServices")]
        internal static MyWebServices WebServices
        {
            [DebuggerHidden]
            get
            {
                return m_MyWebServicesObjectProvider.GetInstance;
            }
        }
    }
}
