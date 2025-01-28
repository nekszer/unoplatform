using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace UnoApp3.MVVM.ViewModels;
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _number = "0";

    [ObservableProperty]
    private string _operations = string.Empty;

    private OperationType LastOperation { get; set; }
    private double PreviousNumber { get; set; }
    private bool Reset { get; set; }

    private string LastOperator = string.Empty;


    [RelayCommand]
    private void Operation(string @operator)
    {
        if (!double.TryParse(Number, out double result))
            return;

        var currentOperator = @operator;
        if (!string.IsNullOrEmpty(LastOperator))
        {
            if (LastOperator != @operator)
                currentOperator = LastOperator;
        }

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
            try
            {
                PreviousNumber = Operate(previous, result, currentOperator);
                Number = GetNumberString(PreviousNumber.ToString());
                Operations = $"{PreviousNumber} {@operator}";
            }
            catch (Exception ex)
            {
                Number = ex.Message;
                Operations = string.Empty;
                PreviousNumber = 0;
                Reset = true;
                LastOperator = string.Empty;
                SetLastOperation(OperationType.Result);
            }
        }
        else
        {
            PreviousNumber = result;
        }

        LastOperator = @operator;
        Reset = true;

        SetLastOperation(OperationType.X_Y);
    }

    private double Operate(double previous, double result, string @operator)
    {
        if (@operator == "+")
            return previous + result;

        if (@operator == "-")
            return previous - result;

        if (@operator == "x")
            return previous * result;

        if (@operator == "/")
        {
            if (result == 0)
                throw new InvalidOperationException("Error, no se puede dividir entre 0");

            return previous / result;
        }

        return 0;
    }

    [RelayCommand]
    private void Square()
    {
        if (!double.TryParse(Number, out double result))
            return;

        Operations = $"sqr({result})";
        var finalResult = result * result;
        PreviousNumber = finalResult;
        Number = GetNumberString(PreviousNumber.ToString());
        Reset = true;

        SetLastOperation(OperationType.Result);
    }

    private double LastValue { get; set; }

    [RelayCommand]
    private void Result()
    {
        if (!double.TryParse(Number, out double result))
            return;
        
        var previous = PreviousNumber;
        Operations = $"{previous} {LastOperator} {result} = ";
        var finalResult = Operate(previous, result, LastOperator);
        PreviousNumber = finalResult;
        Number = GetNumberString(finalResult.ToString());
        Reset = true;

        SetLastOperation(OperationType.Result);
    }

    private void SetLastOperation(OperationType operationType)
    {
        LastOperation = operationType;
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

        if (LastOperation != OperationType.Result)
            return;

        Operations = " ";
        PreviousNumber = 0;

        SetLastOperation(OperationType.Result);
    }

    [RelayCommand]
    private void ClearAll()
    {
        Reset = true;
        Number = "0";

        Operations = " ";
        PreviousNumber = 0;

        SetLastOperation(OperationType.Result);
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

    [RelayCommand]
    private void Percent()
    {
        if (!double.TryParse(Number, out double number))
            return;

        var result = number / 10;
        Number = GetNumberString(result.ToString());

        LastOperator = string.Empty;
        SetLastOperation(OperationType.Result);
    }

    [RelayCommand]
    private void OneOverX()
    {
        if (!double.TryParse(Number, out double number))
            return;

        if(number == 0) 
            return;

        var result = 1D / number;
        Number = GetNumberString(result.ToString());
        Operations = $" 1/{number} = ";

        LastOperator = string.Empty;
        SetLastOperation(OperationType.Result);
    }

    [RelayCommand]
    private void SquareRoot()
    {
        if (!double.TryParse(Number, out double number))
            return;

        var result = Math.Sqrt(number);
        Number = GetNumberString(result.ToString());

        Operations = $" √{number} = ";

        LastOperator = string.Empty;
        SetLastOperation(OperationType.Result);
    }

    private enum OperationType
    {
        X_Y, Result
    }
}
