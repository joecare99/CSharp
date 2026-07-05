using BaseLib.Helper;
using Gen_FreeWin.Data;
using Gen_FreeWin.ViewModels;
using Gen_FreeWin.ViewModels.Interfaces;
using GenFree;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.UI;
using GenFree.Interfaces.VB;
using GenFree.Sys;
using GenFree.ViewModels.Interfaces;
using GenFree.Views;
using GenFree.Models;
using GenFree.Data.DB;
using GenFreeWin.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Gen_FreeWin.Views;
using GenFree.Models.VB;
using GenFree.Interfaces.Model;
using GenFree.Data;
using GenFree.Models.System;

namespace Gen_FreeWin.Main;

[EditorBrowsable(EditorBrowsableState.Never)]
[GeneratedCode("MyTemplate", "8.0.0.0")]
internal class MainApplication : WindowsFormsApplicationBase
{
	private static List<WeakReference> __ENCList = new List<WeakReference>();

	[STAThread]
	internal static void Main(string[] Args)
	{
		try
		{
			Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
			Init(); 
		}
		finally
		{
		}
		MainProject.Application.Run(Args);
	}

	private static void Init()
	{
		var sc= new ServiceCollection()
			.AddSingleton<MainProject>()
			.AddSingleton<MainApplication>()
			.AddSingleton<IModul1>((c)=>_Modul1.Instance)
			.AddSingleton<IUserData, CUserData>()
			.AddOleDB()
			.AddTransient<IMenu1ViewModel, Menu1ViewModel>()
			.AddTransient<ISysTime, CSysTime>()
			.AddSingleton<ISystem, BaseSystem>()
			.AddSingleton<IApplUserTexts, ApplUserTexts>()

			.AddTransient<IStrings, CStrings>()
			.AddTransient<IProjectData, CProjectData>()
			.AddTransient<IOperators, COperators>()
			.AddTransient<IVBInformation, CVBInformation>()
			.AddTransient<IVBConversions, CVBConversions>()
			.AddTransient<IListItem<int>, ListItem<int>>()
			.AddTransient<IInteraction>((c)=>Menue.Default)
			.AddTransient<IGenPersistence, CPersistence>()

			.AddTransient<IAdresseViewModel, AdresseViewModel>()
			.AddTransient<IBesitzRepository, BesitzRepository>()
			.AddTransient<IBesitzViewModel, BesitzViewModel>()
			.AddTransient<IBilderRepository, BilderRepository>()
			.AddTransient<IBilderViewModel, BilderViewModel>()
			.AddTransient<IDubViewModel, DubViewModel>()
			.AddTransient<IFamilieViewModel, FamilieViewModel>()
			.AddTransient<IFraStatisticsViewModel, FrmStatisticsViewModel>()
			.AddTransient<IFraPersImpQueryViewModel, FraPersImpQueryViewModel>()
			.AddTransient<IMandViewModel, MandViewModel>()
			.AddTransient<INamenSuchViewModel, NamenSuchViewModel>()
			.AddTransient<IPersonRedViewModel, PersonRedViewModel>()
			.AddTransient<IPersonenViewModel, PersonenViewModel>()
			.AddTransient<IRepoViewModel, RepoViewModel>()
			.AddTransient<IOFBViewModel, OFBViewModel>()
			.AddTransient<ITextLesenViewModel, TextLesenViewModel>()
			.AddTransient<IVornamViewModel, VornamViewModel>()
			.AddKeyedSingleton<IEventShowEditViewModel, EventShowEditViewModel>(EEventArt.eA_Birth)
			.AddKeyedSingleton<IEventShowEditViewModel, EventShowEditViewModel>(EEventArt.eA_Baptism)
			.AddKeyedSingleton<IEventShowEditViewModel, EventShowEditViewModel>(EEventArt.eA_Death)
			.AddKeyedSingleton<IEventShowEditViewModel, EventShowEditViewModel>(EEventArt.eA_Burial)
			.AddTransient<Personen>()
			.AddTransient<Familie>()
			.AddTransient<Mand>()
			.AddTransient<Textlesen>()
			.AddTransient<Dub>()
			.AddTransient<Quellverw>()
			.AddTransient<Ortsver>()
			.AddTransient<RechText>()
			.AddTransient<Namensuch>()
			.AddTransient<Regsuch>()
			.AddTransient<Partnerrecherche>()
			.AddSingleton<Menue>()
			.AddTransient<Hinter>()
			.AddTransient<Adresse>()
			.AddTransient<Besitz>()
			.AddTransient<Mand>()
			.AddTransient<OFB>()
			.AddTransient<Vornam>()
			.AddTransient<Test1>()
			.AddTransient<Bilder>()
			.BuildServiceProvider();
		IoC.Configure(sc);
	}

	private void MyApplication_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
	{
		Interaction.MsgBox($"{_Modul1.Instance.AppName} ist bereits gestartet!");
	}

	[DebuggerStepThrough]
	public MainApplication()
		: base(AuthenticationMode.Windows)
	{
		base.StartupNextInstance += MyApplication_StartupNextInstance;
		lock (__ENCList)
		{
			__ENCList.Add(new WeakReference(this));
		}
		IsSingleInstance = true;
		EnableVisualStyles = false;
		SaveMySettingsOnExit = true;
		ShutdownStyle = ShutdownMode.AfterMainFormCloses;
	}

	[DebuggerStepThrough]
	protected override void OnCreateMainForm()
	{
		MainForm = MainProject.Forms.Menue;
	}
}
