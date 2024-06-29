# MVVM Tutorials

## Lession 3: The MVVM - Model
```mermaid
flowchart LR;
   subgraph View
    A[View]==>B[Converter];
    A==>C[Validator];
   end
subgraph ViewModel
    D[ViewModel];
end
subgraph Model
    E[Model];
end    
A -- read/write properties --> D
A -- read/execute commands --> D
B -- read/write properties --> D
D -- read/write data --> E
A -- read/write properties --> E
```
## Lession 6: Converters
Part 1 - simple Converter<br />
<img width="270" alt="MVVM_06_Conv1" src="https://raw.githubusercontent.com/joecare99/CSharp/master/CSharpBible/Resources/MVVM_06_Conv1.PNG"><br />
Part 2 - Converter incl. convert back<br />
<img width="270" alt="MVVM_06_Conv2" src="https://raw.githubusercontent.com/joecare99/CSharp/master/CSharpBible/Resources/MVVM_06_Conv2.PNG"><br />
Part 3 - Converter incl. format parameter<br />
<img width="270" alt="MVVM_06_Conv3" src="https://raw.githubusercontent.com/joecare99/CSharp/master/CSharpBible/Resources/MVVM_06_Conv3.PNG"><br />

## Lession 09: DialogBoxes
```mermaid
sequenceDiagram
   autonumber
   actor U as User
    participant V as View
    participant VM as ViewModel
    par View to User
       V ->> +U: Display Form
    and View to ViewModel
       V ->> VM: Create ViewModel
       V ->> VM: Connect to ShowDialog event
    end
    U -->> -V: Press a Button
    V ->> +VM: Execute Button-Command
    VM -->> -V: fire ShowDialog-event
    V ->> +U: Show Dialog to User
    U -->> -V: Exits the Dialog
    V ->> VM: Sends Data from Dialog  
```    
## Lession 16: UserControls
A first glance

## Lession 20: Sysdialogs 
How to open and use (System)-common dialogs

## Lession 21: Buttons 
Use Converter to change the color, and the BaseViewModel.CommandCanExecureBinding to update the Properties. <br />
It's a game: goal to set all buttons to green.<br />
<img width="270" alt="MVVM_21_Buttons" src="https://raw.githubusercontent.com/joecare99/CSharp/master/CSharpBible/Resources/MVVM_21_Buttons.PNG"><br />
    
## Lession 22: WpfCap 
Use Converter to change the color, and the BaseViewModel.CommandCanExecureBinding to update the Properties.<br />
It's a game: goal to sort all colors of the tiles.<br />
<img width="270" alt="MVVM_22_WpfCap" src="https://raw.githubusercontent.com/joecare99/CSharp/master/CSharpBible/Resources/MVVM_22_WpfCap.PNG"><br />

## Lession 24: UserControls extended
Some extended user-controls.

## Lession 27&28: Datagrid
Use a Datagrid to diaplay data
* First: a simple form with a Datagrid
* Second: a datagrid with more complex data
* Third: a custom popup-menu to delete data

## Lession 31: Validation of values
Part 1 - via Exception<br />
<img width="270" alt="MVVM_31_Val1_1" src="https://raw.githubusercontent.com/joecare99/CSharp/master/CSharpBible/Resources/MVVM_31_Val1_1.PNG">
<img width="270" alt="MVVM_31_Val1_2" src="https://raw.githubusercontent.com/joecare99/CSharp/master/CSharpBible/Resources/MVVM_31_Val1_2.PNG"><br />
Part 2 ...

## Lession 33: Events to commands
Use Xaml.Behaviours to use events as commands.
e.g: "got focus" & "lost focus"

## Lession 34: Binding-event Args

## Lession 35: The "CommunityToolkit"
A first use-case

## Lession 36: The "CommunityToolkit" saves work.
An example with a login-Command, a login-Dialog, a "dummy"-userservice.
Showcase of WeakReferenceMessenger and how it makes the life easy.

## Lession 37: Treeview
A book-database with books grouped by category.
Also a showcase for a master-detail-form.

## Lession 38: Dependency injection
small showcase for DI.

## Lession 39: A multi-model-app
An demo-app having 2 models.
showcase for scoped dependency-injection.

## Lession 40: Wizzard
A demo-wizzard app.
With succesive asking the user for information.