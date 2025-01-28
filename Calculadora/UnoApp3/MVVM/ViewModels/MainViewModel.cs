using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace UnoApp3.MVVM.ViewModels;
public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// Propiedad observable para el número actual
    /// </summary>
    [ObservableProperty]
    private string _number = "0";

    /// <summary>
    /// Propiedad observable para las operaciones actuales
    /// </summary>
    [ObservableProperty]
    private string _operations = string.Empty;

    /// <summary>
    /// Tipo de operación anterior
    /// </summary>
    private OperationType LastOperation { get; set; }

    /// <summary>
    /// Número previo a la operación actual
    /// </summary>
    private double PreviousNumber { get; set; } 

    /// <summary>
    /// Indicador para resetear la entrada de números
    /// </summary>
    private bool Reset { get; set; }

    /// <summary>
    /// Operador anterior utilizado
    /// </summary>
    private string LastOperator = string.Empty;

    /// <summary>
    /// Comando para manejar operaciones aritméticas
    /// </summary>
    /// <param name="operator"></param>
    [RelayCommand]
    private void Operation(string @operator)
    {
        if (!double.TryParse(Number, out double result))
            return;

        var currentOperator = DetermineCurrentOperator(@operator); // Determinar operador actual
        UpdateOperationsDisplay(result, currentOperator); // Actualizar la pantalla de operaciones

        if (Reset)
        {
            HandleResetOperation(result, @operator); // Manejar operación de reset
            return;
        }

        HandleCalculation(result, currentOperator, @operator); // Manejar cálculo
        SetLastOperation(OperationType.X_Y); // Establecer la última operación como X_Y
        Reset = true; // Establecer indicador de reset
    }

    /// <summary>
    /// Método para determinar el operador actual
    /// </summary>
    /// <param name="operator"></param>
    /// <returns></returns>
    private string DetermineCurrentOperator(string @operator)
    {
        return !string.IsNullOrEmpty(LastOperator) && LastOperator != @operator ? LastOperator : @operator;
    }

    /// <summary>
    /// Método para actualizar la pantalla de operaciones
    /// </summary>
    /// <param name="result"></param>
    /// <param name="currentOperator"></param>
    private void UpdateOperationsDisplay(double result, string currentOperator)
    {
        Operations = $"{result} {currentOperator}";
    }

    /// <summary>
    /// Método para manejar la operación de reset
    /// </summary>
    /// <param name="result"></param>
    /// <param name="operator"></param>
    private void HandleResetOperation(double result, string @operator)
    {
        Number = GetNumberString(PreviousNumber.ToString());
        LastOperator = @operator;
    }

    /// <summary>
    /// Método para manejar el cálculo de operaciones aritméticas
    /// </summary>
    /// <param name="result"></param>
    /// <param name="currentOperator"></param>
    /// <param name="operator"></param>
    private void HandleCalculation(double result, string currentOperator, string @operator)
    {
        if (PreviousNumber != 0)
        {
            try
            {
                PreviousNumber = Operate(PreviousNumber, result, currentOperator); // Realizar operación
                Number = GetNumberString(PreviousNumber.ToString()); // Actualizar el número actual
                Operations = $"{PreviousNumber} {@operator}"; // Actualizar la operación
            }
            catch (Exception ex)
            {
                HandleOperationException(ex); // Manejar excepción en operación
            }
        }
        else
        {
            PreviousNumber = result; // Establecer número previo al resultado actual
        }

        LastOperator = @operator; // Establecer el operador anterior
    }

    /// <summary>
    /// Método para manejar excepciones en operaciones
    /// </summary>
    /// <param name="ex"></param>
    private void HandleOperationException(Exception ex)
    {
        Number = ex.Message; // Mostrar mensaje de error
        Operations = string.Empty; // Limpiar operaciones
        PreviousNumber = 0; // Reiniciar número previo
        Reset = true; // Establecer indicador de reset
        LastOperator = string.Empty; // Limpiar operador anterior
        SetLastOperation(OperationType.Result); // Establecer última operación como resultado
    }

    /// <summary>
    /// Método para realizar operaciones aritméticas
    /// </summary>
    /// <param name="previous"></param>
    /// <param name="result"></param>
    /// <param name="operator"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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

    /// <summary>
    /// Comando para calcular el cuadrado de un número
    /// </summary>
    [RelayCommand]
    private void Square()
    {
        if (!double.TryParse(Number, out double result))
            return;

        Operations = $"sqr({result})"; // Mostrar operación de cuadrado
        var finalResult = result * result; // Calcular cuadrado
        PreviousNumber = finalResult; // Establecer número previo al resultado final
        Number = GetNumberString(PreviousNumber.ToString()); // Actualizar número actual
        Reset = true; // Establecer indicador de reset

        SetLastOperation(OperationType.Result); // Establecer última operación como resultado
    }

    /// <summary>
    /// Comando para calcular el resultado de la operación actual
    /// </summary>
    [RelayCommand]
    private void Result()
    {
        if (!double.TryParse(Number, out double result))
            return;

        var previous = PreviousNumber;
        Operations = $"{previous} {LastOperator} {result} = "; // Mostrar operación y resultado
        var finalResult = Operate(previous, result, LastOperator); // Calcular resultado final
        PreviousNumber = finalResult; // Establecer número previo al resultado final
        Number = GetNumberString(finalResult.ToString()); // Actualizar número actual
        Reset = true; // Establecer indicador de reset

        SetLastOperation(OperationType.Result); // Establecer última operación como resultado
    }

    /// <summary>
    /// Método para establecer el tipo de última operación
    /// </summary>
    /// <param name="operationType"></param>
    private void SetLastOperation(OperationType operationType)
    {
        LastOperation = operationType;
    }

    /// <summary>
    /// Comando para añadir un dígito al número actual
    /// </summary>
    /// <param name="number"></param>
    [RelayCommand]
    private void AddNumber(string number)
    {
        if (Reset)
        {
            Number = "0";
            Reset = false;
        }

        var previousNumber = Number;
        var numberStr = previousNumber += number;

        Number = GetNumberString(numberStr); // Actualizar número actual
    }

    /// <summary>
    /// Comando para cambiar el signo del número actual
    /// </summary>
    [RelayCommand]
    private void ChangeSign()
    {
        if (!double.TryParse(Number, out double result))
            return;

        if (Number == "0")
            return;

        result *= -1; // Cambiar signo
        Number = GetNumberString(result.ToString()); // Actualizar número actual
        PreviousNumber = result; // Establecer número previo
    }

    /// <summary>
    /// Método para obtener el string del número con el formato adecuado
    /// </summary>
    /// <param name="numberStr"></param>
    /// <returns></returns>
    private string GetNumberString(string numberStr)
    {
        var decimalCount = 0;
        if (numberStr.Contains("."))
            decimalCount = numberStr.Split(".").LastOrDefault().Length;

        if (!double.TryParse(numberStr, out double result))
            return "0";

        return result.ToString($"F{decimalCount}");
    }

    /// <summary>
    /// Comando para limpiar el número actual
    /// </summary>
    [RelayCommand]
    private void ClearNumber()
    {
        Reset = true;
        Number = "0";

        if (LastOperation != OperationType.Result)
            return;

        Operations = " ";
        PreviousNumber = 0;

        SetLastOperation(OperationType.Result); // Establecer última operación como resultado
    }

    /// <summary>
    /// Comando para limpiar todas las operaciones y el número actual
    /// </summary>
    [RelayCommand]
    private void ClearAll()
    {
        Reset = true;
        Number = "0";

        Operations = " ";
        PreviousNumber = 0;

        SetLastOperation(OperationType.Result); // Establecer última operación como resultado
    }

    /// <summary>
    /// Comando para eliminar el último dígito del número actual
    /// </summary>
    [RelayCommand]
    private void RemoveDigit()
    {
        if (Number.Length > 0)
            Number = Number.Remove(Number.Length - 1);

        if (string.IsNullOrEmpty(Number))
            Number = "0";
    }

    /// <summary>
    /// Comando para añadir un punto decimal al número actual
    /// </summary>
    [RelayCommand]
    private void AddDecimalPoint()
    {
        if (string.IsNullOrEmpty(Number))
            Number = "0";

        if (Number.Contains("."))
            return;

        Number += ".";
    }

    /// <summary>
    /// Comando para calcular el porcentaje del número actual
    /// </summary>
    [RelayCommand]
    private void Percent()
    {
        if (!double.TryParse(Number, out double number))
            return;

        var result = number / 10; // Calcular porcentaje (10% en este caso)
        Number = GetNumberString(result.ToString());

        LastOperator = string.Empty;
        SetLastOperation(OperationType.Result); // Establecer última operación como resultado
    }

    /// <summary>
    /// Comando para calcular el inverso del número actual (1/x)
    /// </summary>
    [RelayCommand]
    private void OneOverX()
    {
        if (!double.TryParse(Number, out double number))
            return;

        if (number == 0)
            return;

        var result = 1D / number; // Calcular 1/x
        Number = GetNumberString(result.ToString());
        Operations = $" 1/{number} = ";

        LastOperator = string.Empty;
        SetLastOperation(OperationType.Result); // Establecer última operación como resultado
    }

    /// <summary>
    /// Comando para calcular la raíz cuadrada del número actual
    /// </summary>
    [RelayCommand]
    private void SquareRoot()
    {
        if (!double.TryParse(Number, out double number))
            return;

        var result = Math.Sqrt(number); // Calcular raíz cuadrada
        Number = GetNumberString(result.ToString());

        Operations = $" √{number} = ";

        LastOperator = string.Empty;
        SetLastOperation(OperationType.Result); // Establecer última operación como resultado
    }

    private enum OperationType
    {
        X_Y, Result
    }
}
