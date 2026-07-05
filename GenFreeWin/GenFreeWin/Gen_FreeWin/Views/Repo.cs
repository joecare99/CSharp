using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GenFree.ViewModels.Interfaces;
using Views;

namespace GenFreeWin.Views;

public partial class Repo : Form
{
    private static readonly List<WeakReference> __ENCList = new List<WeakReference>();
    private IRepoViewModel _viewModel;

    public IRepoViewModel ViewModel => _viewModel;

    [DebuggerNonUserCode]
    public Repo(IRepoViewModel viewModel)
    {
        Load += Repo_Load;
        _viewModel = viewModel;
        _viewModel.DoClose = DoClose;
        _viewModel.DoStart = StartProcess;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        InitializeComponent();

        CommandBindingAttribute.Commit(this,_viewModel);
        DblClickBindingAttribute.Commit(this,_viewModel);
        TextBindingAttribute.Commit(this,_viewModel);
        KeyBindingAttribute.Commit(this, _viewModel);
        VisibilityBindingAttribute.Commit(this, _viewModel);
        ListBindingAttribute.Commit(this, _viewModel);
    }

    private void Repo_Load(object sender, EventArgs e)
    {
        _viewModel.FormLoadCommand.Execute(this);
        Font = new Font("Arial", _viewModel.FontSize, FontStyle.Regular);
        BackColor = (Color)_viewModel.HintFarb;
    }

    private void DoClose() => Close();


    private void RichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        _viewModel.LinkClickCommand.Execute(e.LinkText);
    }

    private void StartProcess(string Proc)
    {
        _ = Process.Start(Proc);
    }




}
