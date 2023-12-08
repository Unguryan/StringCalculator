using System.Data;
using System.Text;

//create any expr. that should be computed.
var str = "((2+10*2)(2+3) - 6)/2"; //=52

var resultV1 = CalcV1(str);
var resultV2 = CalcV2(str);

Console.WriteLine("V1: " + resultV1);
Console.WriteLine("V2: " + resultV2);


//Remove spaces, and fix specific case
void PrepareStr(ref string str)
{
    str = str.Replace(" ", "");
    if (str.Contains(")("))
    {
        str = str.Replace(")(", ")*(");
    }
}

//Compute V1, easy one
string CalcV1(string str)
{
    PrepareStr(ref str);
    return new DataTable().Compute(str, string.Empty).ToString();
}

//Compute V2, hard one
string CalcV2(string str)
{
    PrepareStr(ref str);

    if (str.Contains("("))
    {
        while (str.Contains("("))
        {
            var start = str.IndexOf("(");
            while (!char.IsDigit(str[start + 1]))
            {
                start++;
            }

            var temp = -1;
            var end = str.IndexOf(")");
            for (int i = start; i <= end; i++)
            {
                if (str[i] == '(')
                {
                    temp = i;
                }
            }

            if (temp != -1)
            {
                start = temp;
            }

            var sb = new StringBuilder();
            for (int i = start; i <= end; i++)
            {
                sb.Append(str[i]);
            }

            var resp = PreCacl(sb.ToString());
            str = str.Replace(sb.ToString(), resp);
        }
    }

    return PreCacl(str);
}


string PreCacl(string str)
{
    str = str.Replace("(", "").Replace(")", "");
    var operations = new List<string>() { "+", "-", "*", "/" };

    while (operations.Any(str.Contains))
    {
        var indexMul = str.IndexOf("*");
        var indexDiv = str.IndexOf("/");
        var isMulDivIndex = -1;

        if ((indexMul != -1 && indexDiv == -1) || (indexMul != -1 && indexMul < indexDiv))
        {
            isMulDivIndex = indexMul;
        }

        if ((indexDiv != -1 && indexMul == -1) || (indexDiv != -1 && indexDiv < indexMul))
        {
            isMulDivIndex = indexDiv;
        }

        var firstSB = new StringBuilder();
        int index = 0;

        if (isMulDivIndex != -1)
        {
            var sbTemp = new StringBuilder();
            index = --isMulDivIndex;
            while (index >= 0 && char.IsDigit(str[index]))
            {
                sbTemp.Append(str[index]);
                index--;
            }

            firstSB.Append(string.Join(string.Empty, sbTemp.ToString().Reverse()));
        }
        else
        {
            while (index < str.Length && char.IsDigit(str[index]))
            {
                firstSB.Append(str[index]);
                index++;
            }
        }

        var operation = string.Empty;
        if (isMulDivIndex != -1)
        {
            operation = str[++isMulDivIndex].ToString();
        }
        else
        {
            operation = str[index].ToString();
            index++;
        }

        if (isMulDivIndex != -1)
        {
            index = ++isMulDivIndex;
        }

        var secondSB = new StringBuilder();
        while (index < str.Length && char.IsDigit(str[index]))
        {
            secondSB.Append(str[index]);
            index++;
        }

        var strToReplace = firstSB.ToString() + operation + secondSB.ToString();
        var res = Compute(firstSB.ToString(), secondSB.ToString(), operation);

        str = str.Replace(strToReplace, res);
    }

    return str;
}


string Compute(string first, string second, string operation)
{
    var fst = int.Parse(first);
    var snd = int.Parse(second);

    if (operation == "*")
    {
        return (fst * snd).ToString();
    }
    if (operation == "/")
    {
        return (fst / snd).ToString();
    }
    if (operation == "+")
    {
        return (fst + snd).ToString();
    }
    if (operation == "-")
    {
        return (fst - snd).ToString();
    }

    return string.Empty;
}
