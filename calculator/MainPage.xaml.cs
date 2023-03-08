using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text.RegularExpressions;

/////////////////
// Kommentarer //
/////////////////
/*

Detta är mitt första försök på en UWP-app i C#. Har inte programmerat med UI förutom Html, Css.
Kanske tog mig lite vatten över huvudet... :)

Rörde till det lite när jag skulle kunna ha input i form av text från användaren. 
Löste det genom att göra en console till användaren, hoppas detta är ok.
Hurvida metoderna är återanvändningsbara blev med lite krångligare, då jag nog målade in mig i ett hörn lite...
Vissa metoder tycker jag är uppdelade bra, andra inte. Vet att detta är något jag behöver jobba på mer.

Hade även lite problem med testningen då det är en UWP-app, gjorde det lite manuellt.

Har försökt lösa felhanterting efter bästa förmåga då jag inte riktigt vet hur man ska hantera det med UI inputs.
Så kriteriet att programmet inte ska krascha vet jag inte riktigt om det uppfylls. Jag försöker fånga upp felaktig input och kombinationer.

"lista" funktionen gjorde jag som en historik funktion, är det ok? 
Det går även att använda kommandot "lista" men historiken vissas ändå så gör ingen skillnad.

- Patrik

*/

namespace calculator
{

    public sealed partial class MainPage : Page
    {
        //////////////////////
        // GLOBAL VARIABLES //
        //////////////////////

        int whichNumber = 0; // <- KEEP TRACK OF HOW MANY NUMBERS

        List<char> operatorList = new List<char>(); // <- STORE OPERATORS TO USE IN CALC
        List<string> inputStringList = new List<string>(); // <- STORE THE WHOLE CALC FOR HISTORY
        List<double> inputNumberList = new List<double>(); // <- STORE NUMBERS TO USE IN CALC
        List<double> resultList = new List<double>(); // <- STORE PREVIOUS RESULTS 

        List<string> historyList = new List<string>(); // <- STORY PREVIOUS CALCULATIONS

        string tempString = ""; // <- STORE PREVIOUS NUMBERS FOR "CACHE" PORPUSE
        string lastInput = ""; // <- STORE PREVIOUS INPUT VALUE
        char op; // <- STORE CURRENT OPERATOR

        double result; // <- STORE CURRENT RESULT

        string userCode; // <- STORE USER INPUT VIA CONSOLE

        bool newton = false; // <- IS NEWTON COMMAND ACTIVE?

        //////////////////////
        //////////////////////
        //////////////////////

        public MainPage()
        {
            this.InitializeComponent();
        }

        // RESETS MAIN VARIABELS TO INITIAL STATE //
        private void ResetVariabels()
        {
            whichNumber = 0;
            operatorList.Clear();
            inputStringList.Clear();
            inputNumberList.Clear();
            tempString = "";
            op = 'a';
            result = 0;
            userCode = "";
            lastInput = "";

        }

        // HISTORY BUTTON CLICK //
        private void History_Click(object sender, RoutedEventArgs e)
        {
            ShowHistory();

        }

        /// <summary>
        /// Outputs history list on history UI box
        /// </summary>
        private void ShowHistory()
        {
            HistoryBox.Text = ""; // <- CLEAR HISTORY TEXTBOX

            // Loop through list in reverse order //
            for (int i = historyList.Count - 1; i >= 0; i--)
            {
                HistoryBox.Text += historyList[i] + "\n";
            }


        }

        /// <summary>
        /// Clears history list and history UI box
        /// </summary>
        /// <param name="sender">Clear button</param>
        /// <param name="e"></param>
        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            HistoryBox.Text = ""; // <- CLEAR HISTORY TEXTBOX
            historyList.Clear();

        }

        /// <summary>
        /// Main numbers UI button click handler. Sends input from buttons to correct method
        /// </summary>
        /// <param name="sender">clicked button</param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) 
        {
            Button b = sender as Button;
            string tempUserInput = b.Content.ToString();

            userCode = ""; // <- CLEAR USER CODE 

            // SEND INPUT VALUE FOR CHECKING //
            if (!newton) {
                CheckInputs(tempUserInput);

            }
            else
            {
                CheckInputsNewton(tempUserInput);

            }

            lastInput = tempUserInput;

            //DebugMetod();

        }


        /// <summary>
        /// Takes a string input of type number or operator and returns a calculated value.
        /// Uses the Calculate() method
        /// </summary>
        /// <param name="tempUserInput">String (0-9, /, x, -, +, =, CE)</param>
        /// <returns>The result of the calculation</returns>
        private double CheckInputs(string tempUserInput)
        {
            bool isNumber; // <- KEEP TRACK OF INPUT IS A NUMBER OR STRING
            string tempHistoryString; // <- TEMP HISTORY FOR STORING PORPUSE

            double _calculatedResult = 0; // <- TEMP RESULT - RETURN VALUE 

            // CHECK IF INPUT IS NUMBER/"." //
            if (new string[] { "/", "x", "-", "+", "=", "CE" }.Contains(tempUserInput))
            {
                isNumber = false;
            }
            else
            {
                isNumber = true;
            }

            // IF NOTHING IN VARIABLES = UNSURE IF CALCULATION IS DONE//
            if (tempUserInput == "=" && whichNumber == 0)
            {
                // IF SOMETHING IN TEMP NUMBER = CALCULTAION NOT FINISHED//
                if (tempString != "")
                {
                    // USE TEMP NUMBER AS RESULT //
                    _calculatedResult = Convert.ToDouble(tempString); 
                    resultList.Add(_calculatedResult);
                    inputBox.Text = Convert.ToString(_calculatedResult); // <- DISPLAY IN UI

                }
                else {
                    if (resultList.Count > 0)
                    {
                        // IF NO CURRENT CALCULATION USE LAST RESULT //
                        _calculatedResult = resultList[resultList.Count-1];
                    }

                }

                ShowHistory(); // <-- DISPLAY HISTORY IN UI
                ResetVariabels(); // <-- RESET VARIABLES

            }

            ///////////////////
            // HANDLE INPUTS //
            ///////////////////

            // IF NUMBER BUT NOT "." //
            if (isNumber == true && tempUserInput != ".")
            {
                // IF NEW CALCULATION -> RESET RESULT IN UI //
                if (whichNumber == 0)
                {
                    OutputBox.Text = ""; // -> RESET RESULT IN UI 
                }

                tempString += tempUserInput; // <- ADD CURRENT INPUT TO TEMP NUMBER

                // DISPLAY IN UI //
                inputBox.Text = "";
                for (int i = 0; i < inputStringList.Count; i++)
                {
                    inputBox.Text += inputStringList[i];
                }
                inputBox.Text += tempString;

            }
            // IF INPUT = "." -> CHECK IF NUMBER CONTAINS "." //
            else if(isNumber == true && tempUserInput == "." && lastInput != "/" && lastInput != "x" && lastInput != "-" && lastInput != "+")
            {
                // CHECK IF DECIMAL //
                if (!tempString.Contains("."))
                {
                    // IF NOTHING ADD ZERO TO START OF NUMBER //
                    if (tempString == "")
                    {
                        tempString += "0";

                    }

                    tempString += tempUserInput; // <- ADD CURRENT INPUT TO TEMP NUMBER

                    // DISPLAY IN UI //
                    inputBox.Text = "";
                    for (int i = 0; i < inputStringList.Count; i++)
                    {
                        inputBox.Text += inputStringList[i];
                    }
                    inputBox.Text += tempString;

                }

            }
            // NOT NUMBER OR "." //
            else 
            {
                // IF OPERATOR //
                if (new string[] { "/", "x", "-", "+" }.Contains(tempUserInput) && tempString != "" && lastInput != "." && lastInput != "/" && lastInput != "x" && lastInput != "-" && lastInput != "+" && lastInput != "")
                {

                    // ADD CURRENT INPUT //
                    op = Convert.ToChar(tempUserInput);
                    inputNumberList.Add(Convert.ToDouble(tempString));
                    inputStringList.Add(tempString);
                    inputStringList.Add(Convert.ToString(op));
                    operatorList.Add(op);
                    tempString = "";

                    whichNumber++; // <- KEEP TRACK OF HOW MANY NUMBERS

                    // DISPLAY IN UI //
                    inputBox.Text = "";
                    for (int i = 0; i < inputStringList.Count; i++)
                    {
                        inputBox.Text += inputStringList[i];
                    }

                }
                // IF EQUALS //
                else if (tempUserInput == "=")
                {
                    if (whichNumber != 0 && lastInput != "." && lastInput != "/" && lastInput != "x" && lastInput != "-" && lastInput != "+" && lastInput != "=" && lastInput != "")
                    {
                        // ADD CURRENT INPUT //
                        op = Convert.ToChar(tempUserInput);
                        inputNumberList.Add(Convert.ToDouble(tempString));
                        inputStringList.Add(tempString);
                        inputStringList.Add(Convert.ToString(op));
                        operatorList.Add(op);
                        tempString = "";

                        // CALCULATE RESULT //
                        result = Calculate(operatorList, inputNumberList);
                        _calculatedResult = result;
                        resultList.Add(_calculatedResult);

                        OutputBox.Text = result.ToString(); // <-- DISPLAY RESULT IN UI

                        // ADD TO HISTORY LIST //
                        tempHistoryString = "";
                        for (int i = 0; i < inputStringList.Count; i++)
                        {
                            tempHistoryString = tempHistoryString + inputStringList[i];

                        }
                        tempHistoryString = tempHistoryString + result;
                        historyList.Add(tempHistoryString);

                        ShowHistory(); // <-- DISPLAY HISTORY IN UI
                        ResetVariabels(); // <-- RESET VARIABLES

                    }
                    // IF PREVIOUS INPUT "/", "x", "-", "+" -> CORRECT PREVIOUS INPUT // 
                    else if (new string[] { "/", "x", "-", "+" }.Contains(lastInput))
                    {
                        operatorList.RemoveAt(operatorList.Count - 1); // <- REMOVE LAST ENTRY
                        inputStringList.RemoveAt(inputStringList.Count - 1); // <- REMOVE LAST ENTRY

                        // CORRECT IN UI INPUT DISPLAY //
                        inputBox.Text = "";
                        for (int i = 0; i < inputStringList.Count; i++)
                        {
                            inputBox.Text += inputStringList[i];
                        }

                        // ADD CURRENT INPUT //
                        op = Convert.ToChar(tempUserInput);

                        inputStringList.Add(Convert.ToString(op));
                        operatorList.Add(op);
                        tempString = "";

                        // CALCULATE RESULT //
                        result = Calculate(operatorList, inputNumberList);
                        _calculatedResult = result;
                        resultList.Add(_calculatedResult);

                        OutputBox.Text = result.ToString(); // <-- DISPLAY RESULT IN UI

                        //////////////////////////
                        // ADD TO HISTORY LIST //
                        tempHistoryString = "";

                        for (int i = 0; i < inputStringList.Count; i++)
                        {
                            tempHistoryString = tempHistoryString + inputStringList[i];

                        }

                        tempHistoryString = tempHistoryString + result;
                        historyList.Add(tempHistoryString);
                        //////////////////////////

                        ShowHistory(); // <-- DISPLAY HISTORY IN UI
                        ResetVariabels(); // <-- RESET VARIABLES

                    }

                }
                else if (tempUserInput == "CE")
                {
                    // IF CE/RESET //
                    inputBox.Text = "0"; // <-- RESET INPUT IN UI
                    OutputBox.Text = ""; // <-- RESET OUTPUT IN UI
                    DebugConsole.Text = "DEBUG"; // <-- RESET DEBUG IN UI

                    ResetVariabels(); // <-- RESET VARIABLES

                }

            }

            // DISABLE/ENABLE EQUAL BUTTON //
            if (inputNumberList.Count > 0 && tempString != "")
            {
                Button_equals.IsEnabled = true;
            }
            else if (inputNumberList.Count > 1)
            {
                Button_equals.IsEnabled = true;
            }
            else
            {
                Button_equals.IsEnabled = false;
            }

            return _calculatedResult;

        } // <- CheckInputs END! //

        /// <summary>
        /// Calculate - Takes a list of operators and numbers and returns the result
        /// </summary>
        /// <param name="_operatorList">A list of operators</param>
        /// <param name="_inputNumberList">A list of doubles</param>
        /// <returns>The result of the calculation</returns>
        public static double Calculate( List<char> _operatorList, List<double> _inputNumberList )
        {
            
            double _result = 0;

            if (_inputNumberList.Count > 0)
            {
                _result = _inputNumberList[0];

            }
            

            if (_operatorList.Count > 0)
            {
                for (int i = 0; i < _operatorList.Count; i++)
                {

                    if (_operatorList[i] == '/')
                    {
                        _result = _result / _inputNumberList[i + 1];

                    }
                    else if (_operatorList[i] == 'x')
                    {
                        _result = _result * _inputNumberList[i + 1];

                    }
                    else if (_operatorList[i] == '-')
                    {
                        _result = _result - _inputNumberList[i + 1];

                    }
                    else if (_operatorList[i] == '+')
                    {
                        _result = _result + _inputNumberList[i + 1];

                    }

                }
            }
            
            return _result;

        } // <- Calculate END! //

        // START OF NEWTON COMMAND // 
        private void Newton()
        {
            newton = true;

            ResetVariabels();
            ConsoleBox.Text = "";
            inputBox.Text = "0";
            OutputBox.Text = "";

            instrTextBlock.Text = "Input Mass:"; // <- USER INSTRUCTIONS

            // DISABLE/ENABLE BUTTONS //
            Button_multiply.IsEnabled = true;
            Button_subtract.IsEnabled = false;
            Button_divide.IsEnabled = false;
            Button_add.IsEnabled = false;
            Button_CtoF.IsEnabled = false;
            Button_FtoC.IsEnabled = false;
            Button_equals.IsEnabled = false;


            AlertBox("Newton command.\n(Mass x Acceleration = Force)");


        } // <- Newton END! //

        /// <summary>
        /// Takes a string input of type number or operator and calculates the value of m*a=F.
        /// Uses the Calculate() method
        /// </summary>
        /// <param name="tempUserInput">String (0-9, /, x, -, +, =, CE)</param>
        private void CheckInputsNewton(string tempUserInput)
        {
            bool isNumber; // <- KEEP TRACK OF INPUT IS A NUMBER OR STRING

            string tempHistoryString;

            double _calculatedResult = 0;

            // CHECK IF INPUT IS NUMBER/"." //
            if (new string[] { "/", "x", "-", "+", "=", "CE" }.Contains(tempUserInput))
            {
                isNumber = false;
            }
            else
            {
                isNumber = true;
            }

            // IF NUMBER //
            if (isNumber == true && tempUserInput != ".")
            {
                // 1 = MASS //
                if (whichNumber < 2)
                {
                    tempString += tempUserInput; // <- ADD CURRENT INPUT TO TEMP NUMBER
                    inputBox.Text = tempString; // <- DISPLAY NUMBER IN UI
                    whichNumber = 1;


                }
                // 2 = ACCELERATION //
                else if (whichNumber == 2)
                {
                    tempString += tempUserInput; // <- ADD CURRENT INPUT TO TEMP NUMBER
                    inputBox.Text = tempString; // <- DISPLAY NUMBER IN UI

                }

            }
            // IF INPUT = "." -> CHECK IF NUMBER CONTAINS DECIMAL //
            else if (isNumber == true && tempUserInput == "." && lastInput != "x")
            {
                // CHECK IF DECIMAL //
                if (!tempString.Contains("."))
                {
                    // IF TEMP NUMBER EMPTY //
                    if (tempString == "")
                    {
                        tempString += "0"; // <- ADD ZERO AT START OF NUMBER

                    }

                    // IF NUMBER //
                    if (whichNumber < 2)
                    {
                        tempString += tempUserInput; // <- ADD CURRENT INPUT TO TEMP NUMBER
                        inputBox.Text = tempString; // <- DISPLAY NUMBER IN UI

                    }
                    else if (whichNumber == 2)
                    {
                        tempString += tempUserInput; // <- ADD CURRENT INPUT TO TEMP NUMBER
                        inputBox.Text = tempString; // <- DISPLAY NUMBER IN UI

                    }

                }

            }
            // IF OPERAND //
            else
            {
                if (tempUserInput == "x" || tempUserInput == "=")
                {
                    // IF MASS -> HANDLE STORING INPUT //
                    if (whichNumber == 1)
                    {
                        // STORE NUMBER AS STRING //
                        inputStringList.Add(tempString);

                        // STORE OPERAND //
                        op = 'x';
                        operatorList.Add(op);
                        inputStringList.Add(Convert.ToString(op));

                        // STORE NUMBER AS DOUBLE //
                        inputNumberList.Add(Convert.ToDouble(tempString));  
                        
                        tempString = ""; // <- RESET TEMP NUMBER = NEW NUMBER

                        inputBox.Text = "0"; // <- RESET INPUT IN UI
                        instrTextBlock.Text = "Input Acceleration:"; // <- USER INSTRUCTIONS
                        whichNumber = 2;

                        // DISABLE/ENABLE BUTTONS //
                        Button_equals.IsEnabled = true;
                        Button_multiply.IsEnabled = false;

                    }
                    // IF FORCE -> SHOW RESULT //
                    else if (whichNumber == 2 && tempString != "")
                    {
                        
                        inputNumberList.Add(Convert.ToDouble(tempString));
                        inputStringList.Add(tempString);
                        tempString = "";

                        // CALCULATE RESULT //
                        result = Calculate(operatorList, inputNumberList);
                        _calculatedResult = result;
                        resultList.Add(_calculatedResult);

                        // SHOW CALCULATION IN UI //
                        inputBox.Text = inputStringList[0] + "x" + inputStringList[2];
                        OutputBox.Text = "Force = " + result.ToString();

                        // ADD CALCULATION TO HISTORY LIST //
                        tempHistoryString = "";
                        tempHistoryString = inputStringList[0] + "(m) x " + inputStringList[2] + "(a) = " + result + "(F)";
                        historyList.Add(tempHistoryString);

                        ShowHistory(); // <-- DISPLAY HISTORY IN LIST
                        ResetVariabels(); // <-- RESET VARIABLES

                        newton = false; // <- END OF NEWTON COMMAND

                        // DISABLE/ENABLE BUTTONS //
                        Button_subtract.IsEnabled = true;
                        Button_divide.IsEnabled = true;
                        Button_multiply.IsEnabled = true;
                        Button_add.IsEnabled = true;
                        Button_CtoF.IsEnabled = true;
                        Button_FtoC.IsEnabled = true;
                        Button_equals.IsEnabled = false;

                    }

                }
                else if (tempUserInput == "CE")
                {
                    // IF RESET //
                    inputBox.Text = "0"; // <- RESET INPUT IN UI
                    OutputBox.Text = ""; // <- RESET OUTPUT IN UI

                    ResetVariabels(); // <- RESET VARIABLES
                    Newton(); // <- RESTART NEWTON COMMAND

                }

            }


        } // <- CheckInputsNewton END! //

        // CECLIUS <-> F CONVERT BUTTON CLICK // -> handleCtoF
        private void CtoF_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            HandleCtoF(b.Name);

        }

        /// <summary>
        /// Handles C <-> F convert. Takes name of button clicked and shows result of C <-> F conversion on display.
        /// Uses CtoF() method
        /// </summary>
        /// <param name="clickedName">Button_CtoF or Button_FtoC</param>
        private void HandleCtoF(string clickedName)
        {
            double convertResult = 0; // <- RESULT OF CONVERSION
            double tempCFResult = 0; // <- TEMP RESULT OF CALCULATION
            string sign1; // <- °C OR °F FOR DISPLAY PURPOSE
            string sign2; // <- °C OR °F FOR DISPLAY PURPOSE

            string convertType; // <- KEEP TRACK OF °C OR °F FOR CONVERTION PURPOSE

            // SET CONVERT TYPE
            if (clickedName == "Button_CtoF")
            {
                convertType = "c";
                sign1 = "°C";
                sign2 = "°F";
            }
            else
            {
                convertType = "f";
                sign1 = "°F";
                sign2 = "°C";

            }

            tempCFResult = CheckInputs("="); // <- CALCULATE A RESULT TO CONVERT
            convertResult = CtoF(tempCFResult, convertType); // <- CONVERT

            // ADD TO HISTORY //
            historyList.Add(Convert.ToString(tempCFResult) + sign1 + " = " + Convert.ToString(convertResult) + sign2);
            
            // DISPLAY RESULT IN UI //
            OutputBox.Text = Convert.ToString(convertResult + sign2);

            ShowHistory(); // <-- DISPLAY HISTORY IN LIST
            ResetVariabels(); // <-- RESET VARIABLES

        }

        /// <summary>
        /// Takes a double and a string of "c" or "f" and returns converted value
        /// </summary>
        /// <param name="number">double in celsius or fahrenheit</param>
        /// <param name="type"></param>
        /// <returns>double C <-> F </returns>
        public static double CtoF(double number, string type)
        {

            double convertResult; // <- TEMP CONVERTION RESULT = RETURN VALUE

            if (type == "c")
            {
                // F -> C // (x-32)*0.5556
                convertResult = (number * 1.8) + 32;

            }
            else if (type == "f")
            {
                // C -> F // (x*1.8)-32
                convertResult = (number - 32) * 0.5556;

            }
            else
            {
                convertResult = 0;

            }

            return convertResult;

        }

        // USER CONSOLE //
        //////////////////
        // TAKES INPUT FROM THE USER VIA TEXT //

        /// <summary>
        /// Takes input from the user console
        /// </summary>
        /// <param name="sender">keyboard</param>
        /// <param name="e">input character</param>
        private void Console_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            string pressedKey = e.Key.ToString();

            CheckKey(pressedKey);
        }

        /// <summary>
        /// Checks command sent via user console. Stores and combines them.
        /// </summary>
        /// <param name="pressedKey">input character</param>
        private void CheckKey(string pressedKey)
        {

            if (pressedKey == "Enter")
            {
                userCode = ConsoleBox.Text; // <- INPUT FROM USER CONSOLE

                if (userCode == "MARCUS" || userCode == "marcus" || userCode == "Marcus")
                {
                    if (!newton)
                    {
                        tempString = ""; // <- RESET CURRENT NUMBER IN CALC
                        CheckInputs("42"); // <- SEND NUMBER TO CALCULATION
                    }
                    else {
                        tempString = ""; // <- RESET CURRENT NUMBER IN CALC
                        CheckInputsNewton("42"); // <- SEND NUMBER TO CALCULATION
                    }
                    
                    userCode = ""; // <- RESET USER COMMAND
                    ConsoleBox.Text = "";  // <- RESET USER CONSOLE UI

                    AlertBox("Secret code 42!!!"); // <- INFORM USER WITH ALERTBOX

                }
                else if (userCode == "NEWTON" || userCode == "newton" || userCode == "Newton")
                {

                    userCode = ""; // <- RESET USER COMMAND
                    Newton();

                }
                else if (userCode == "LISTA" || userCode == "lista" || userCode == "Lista")
                {
                    ShowHistory(); // <- DISPLAY HISTORY IN LIST
                    userCode = ""; // <- RESET USER COMMAND
                    ConsoleBox.Text = ""; // <- RESET USER CONSOLE UI

                }
                else if (userCode == "QUIT" || userCode == "quit" || userCode == "Quit")
                {
                    CloseApp();
                }
                else
                {
                    userCode = ""; // <- RESET USER COMMAND
                    ConsoleBox.Text = ""; // <- RESET USER CONSOLE UI

                }

            }

        } // <- CheckKey END! //

        /// <summary>
        /// Displays Alert Box to user
        /// </summary>
        /// <param name="alertBoxText">string to display</param>
        private async void AlertBox(string alertBoxText)
        {
            ContentDialog newDialog = new ContentDialog
            {
                Title = "Alert",
                Content = alertBoxText,
                CloseButtonText = "Close"
            };

            ContentDialogResult result = await newDialog.ShowAsync();
        }

        // EXIT APPLICATION //
        public void CloseApp()
        {
            Application.Current.Exit();
        }

        // FOR DEBUGGING //
        ///////////////////
        private void DebugMetod() {
            DebugConsoleBorder.Visibility = 0;

            /////////////////////
            // DEBUG LASTINPUT //
            /*
            
            DebugConsole.Text += "\nLast input: " + lastInput;
            DebugConsole.Text += "\nWhichnumber: " + whichNumber;
            */
            // DEBUG LASTINPUT //
            /////////////////////

            
            DebugConsole.Text = "Listor";

            DebugConsole.Text += "\n INTS:";
            for (int i = 0; i < inputNumberList.Count; i++)
            {
                DebugConsole.Text += "\n" + inputNumberList[i];
            }

            DebugConsole.Text += "\n STRINGS:";
            for (int i = 0; i < inputStringList.Count; i++)
            {
                DebugConsole.Text += "\n" + inputStringList[i];
            }

            DebugConsole.Text += "\n OPERATORS:";
            for (int i = 0; i < operatorList.Count; i++)
            {
                DebugConsole.Text += "\n" + operatorList[i];
            }
            



        } // <- DebugMetod END! //








        //! END OF CLASS !//
    }
}
