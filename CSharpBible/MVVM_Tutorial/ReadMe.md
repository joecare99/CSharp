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

## Lession 31: Validation of values
Part 1 - via Exception<br />
<img width="270" alt="MVVM_31_Val1_1" src="https://raw.githubusercontent.com/joecare99/CSharp/master/CSharpBible/Resources/MVVM_31_Val1_1.PNG">
<img width="270" alt="MVVM_31_Val1_2" src="https://raw.githubusercontent.com/joecare99/CSharp/master/CSharpBible/Resources/MVVM_31_Val1_2.PNG"><br />
Part 2 ...
