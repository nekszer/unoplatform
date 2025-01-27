using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace UnoApp3.MVVM.ViewModels;
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _number = "0";

    [ObservableProperty]
    private string _operations = "";

    private OperationType LastOperation { get; set; }
    private OperationType CurrentOperation { get; set; }
    private double PreviousNumber { get; set; }
    private bool Reset { get; set; }

    private string LastOperator = "";

    [RelayCommand]
    private void Operation(string @operator)
    {
        if (!double.TryParse(Number, out double result))
            return;

        LastOperation = CurrentOperation;
        CurrentOperation = OperationType.X_Y;

        var currentOperator = @operator;
        if (LastOperator != @operator)
            currentOperator = LastOperator;

        Operations = $"{result} {currentOperator}";

        if (Reset)
        {
            Number = GetNumberString(PreviousNumber.ToString());
            LastOperator = @operator;
            return;
        }

        var previous = PreviousNumber;
        if (previous != 0)
        {
            PreviousNumber = Operate(previous, result, currentOperator);
            Number = GetNumberString(PreviousNumber.ToString());
            Operations = $"{PreviousNumber} {@operator}";
        }
        else
        {
            PreviousNumber = result;
        }

        LastOperator = @operator;
        Reset = true;
    }

    private double Operate(double previous, double result, string @operator)
    {
        if (@operator == "+")
            return previous + result;

        if(@operator == "-")
            return previous - result;

        if(@operator == "x") 
            return previous * result;

        if(@operator == "/")
            return previous / result;

        return 0;
    }

    [RelayCommand]
    private void Square()
    {
        if (!double.TryParse(Number, out double result))
            return;

        LastOperation = CurrentOperation;
        CurrentOperation = OperationType.Result;
        Operations = $"sqr({result})";
        var finalResult = result * result;
        PreviousNumber = finalResult;
        Number = GetNumberString(PreviousNumber.ToString());
        Reset = true;
    }

    [RelayCommand]
    private void Result()
    {
        if (!double.TryParse(Number, out double result))
            return;

        LastOperation = CurrentOperation;
        CurrentOperation = OperationType.Result;

        var previous = PreviousNumber;
        Operations = $"{Operations} {result} = ";

        var finalResult = Operate(previous, result, LastOperator);
        PreviousNumber = finalResult;
        Number = GetNumberString(finalResult.ToString());
        Reset = true;
    }

    [RelayCommand]
    private void AddNumber (string number)
    {
        if (Reset)
        {
            Number = "0";
            Reset = false;
        }

        var previousNumber = Number;
        var numberStr = previousNumber += number;

        Number = GetNumberString(numberStr);
    }

    [RelayCommand]
    private void ChangeSign ()
    {
        if (!double.TryParse(Number, out double result))
            return;

        if (Number == "0")
            return;

        result *= -1;
        Number = GetNumberString(result.ToString());
        PreviousNumber = result;
    }

    private string GetNumberString(string numberStr)
    {
        var decimalCount = 0;
        if (numberStr.Contains("."))
            decimalCount = numberStr.Split(".").LastOrDefault().Length;

        if (!double.TryParse(numberStr, out double result))
            return "0";

        return result.ToString($"F{decimalCount}");
    }

    [RelayCommand]
    private void ClearNumber ()
    {
        Reset = true;
        Number = "0";

        if (CurrentOperation != OperationType.Result)
            return;

        Operations = " ";
        PreviousNumber = 0;
        LastOperation = OperationType.Result;
        CurrentOperation = OperationType.Result;
    }

    [RelayCommand]
    private void ClearAll()
    {
        Reset = true;
        Number = "0";

        Operations = " ";
        PreviousNumber = 0;
        LastOperation = OperationType.Result;
        CurrentOperation = OperationType.Result;
    }

    [RelayCommand]
    private void RemoveDigit()
    {
        if (Number.Length > 0)
            Number = Number.Remove(Number.Length - 1);

        if(string.IsNullOrEmpty(Number))
            Number = "0";
    }

    [RelayCommand]
    private void AddDecimalPoint()
    {
        if (string.IsNullOrEmpty(Number))
            Number = "0";

        if (Number.Contains("."))
            return;

        Number += ".";
    }

    private enum OperationType
    {
        X_Y, Result
    }
}
