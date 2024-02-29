using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MSQBrowser.Properties;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using BaseLib.Helper.MVVM;
using BaseLib.Helper;
using MySqlConnector;
using System.Data;
using System.Net;

namespace MSQBrowser.ViewModels;

[NotifyDataErrorInfo]
public partial class DBConnectViewModel : BaseViewModelCT
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TTServer))]
    [NotifyCanExecuteChangedFor(nameof(ListDBSCommand))]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    [Required(ErrorMessageResourceName =nameof(Resources.Err_Required),ErrorMessageResourceType =typeof(Resources))]
    private string _server = "localhost";
    public string? TTServer => this.ValidationText();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TTUser))]
    [NotifyCanExecuteChangedFor(nameof(ListDBSCommand))]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    [Required(ErrorMessageResourceName = nameof(Resources.Err_Required), ErrorMessageResourceType = typeof(Resources))]
    private string _user = "";
    public string? TTUser => this.ValidationText();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TTPassword))]
    [NotifyCanExecuteChangedFor(nameof(ListDBSCommand))]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    [Required(AllowEmptyStrings =true, ErrorMessageResourceName = nameof(Resources.Err_Required), ErrorMessageResourceType = typeof(Resources))]
    private SecureString _password;
    public string? TTPassword => this.ValidationText();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TTDb))]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    [Required(ErrorMessageResourceName = nameof(Resources.Err_Required), ErrorMessageResourceType = typeof(Resources))]
    private string _db;
    public string? TTDb => this.ValidationText();

    [ObservableProperty]
    [CustomValidation(typeof(DBConnectViewModel),nameof(NoValidator))] 
    private IEnumerable<string>? _DBs;

    [ObservableProperty]
    [CustomValidation(typeof(DBConnectViewModel), nameof(NoValidator))]
    private string _testMsg="";

    public Action<object?>? OnAccept { get; set; }

    public DBConnectViewModel()
    {
        // The Initialization
    }

    public static ValidationResult NoValidator(object value, ValidationContext context)
    {
        return ValidationResult.Success;
    }

    public bool CanListDBs()
    {
        return string.IsNullOrEmpty(TTServer) && string.IsNullOrEmpty(TTUser) && string.IsNullOrEmpty(TTPassword);
    }

    [RelayCommand(CanExecute =nameof(CanListDBs))]
    private void ListDBS()
    {
        var nwc= new NetworkCredential(User, Password);
        var con = new MySqlConnection() { ConnectionString = new MySqlConnectionStringBuilder() 
        { Server = Server, UserID = nwc.UserName, Password = nwc.Password, Database = "mysql", CharacterSet = "UTF8" }.ConnectionString };
        try
        {
            try
            {
                con.Open();
                var db = con.GetSchema("Databases");
                DBs = db.Rows.Cast<DataRow>().Select(o => o["SCHEMA_NAME"].ToString()).ToList();
            }
            finally { 
                con.Close(); 
            }
        }
        catch(Exception ex) 
        { 
            System.Diagnostics.Debug.WriteLine(ex.Message); 
        }
    }

    [RelayCommand]
    private void ExecutePasswordChanged(SecureString obj)
    {
        if (obj != null)
            Password = obj;
    }

    [RelayCommand]
    private void Test() {
        var nwc = new NetworkCredential(User, Password);
        var con = new MySqlConnection()
        {
            ConnectionString = new MySqlConnectionStringBuilder()
            { Server = Server, UserID = nwc.UserName, Password = nwc.Password, Database = Db, CharacterSet = "UTF8" }.ConnectionString
        };
        try
        {
            try
            {
                con.Open();
                TestMsg = Resources.ConnectedTo.Format(con.ServerVersion);
            }
            finally
            {
                con.Close();
            }
        }
        catch (Exception ex)
        {
            TestMsg = Resources.TestNotSuccessful;
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }
    private bool CanConnect()=>
        TTServer == null && TTUser == null && TTPassword == null && TTDb == null;

    [RelayCommand(CanExecute =nameof(CanConnect))]
    private void Connect()
    {
        OnAccept?.Invoke(this);
    }


}
