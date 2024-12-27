using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System;
using System.Collections;

string path = @"C:\Extra\Projects\mule\inputxd.txt";
Console.WriteLine("---Program starts---");

var lines = File.ReadAllLines(path);

//directions 0 up 1 left 2 bottom 3 right
//calculate start
var startY = 0;
var startX = 0;
for(int i=0; i<lines.Length;i++)
{
    for(int j=0; j<lines[0].Length;j++)
    {
        if(lines[i][j]== '^')
        {
            startY=i;
            startX=j;
        }
    }
}
Console.WriteLine($"Start pos: {startY} {startX}");
int [] myPos = { startY, startX, 0};
char[][] linesArray = new char[lines.Length] [];
for(int i = 0 ; i < lines.Length; i++)
{
    linesArray[i]=lines[i].ToCharArray();
}


//initial ^ place
var sum= 1;
var cyclesNumber = 0;

ArrayList pathY = new ArrayList();
ArrayList pathX = new ArrayList();
for(int i = 0;; i++)
{
    var temp = makeMove(myPos, linesArray, true, pathY, pathX);
    if(temp == -2)
    {
        //znalazl cykla
        cyclesNumber++;
    }
    if(temp<0){
        break;
    }
    sum+=temp;
    //printMaze(linesArray);
}

//linesArray[67][102]= '█';
printMaze(linesArray);
//TestPath(pathY,pathX);

Console.WriteLine(sum);
if(sum != pathY.Count){
    Console.WriteLine($"Olaboga o matko i curko sum: {sum}, pathY.Cont:{pathY.Count}");
    throw new ArithmeticException();
}

if(sum != 5086){
    // that was good answer
    Console.WriteLine($"Olaboga o matko i curko sum: {sum}");
    throw new ArithmeticException();
}
TestPath(pathY,pathX);
Console.WriteLine(cyclesNumber);

Console.WriteLine($"Path length is: {pathY.Count}");
//TestPath(pathY,pathX);
foreach(var elt in pathY)
{
    Console.Write($"{elt}, ");
}
Console.WriteLine();
foreach(var elt in pathX)
{
    Console.Write($"{elt}, ");
}

//wystarczy w pathX i pathY pobierać od 1indexu nie od 0
for(int i = 1; i<pathY.Count;i++)
{
    char[][] linesArrayCopy = new char[lines.Length] [];
    for(int j = 0 ; j < lines.Length; j++)
    {
        linesArrayCopy[j]=lines[j].ToCharArray();
    }
    //Console.WriteLine($"Y: {(int)pathY[i]}, X: {(int)pathX[i]}");
    //Console.WriteLine($"My var is: {linesArrayCopy} ---V");
    //printMaze(linesArrayCopy);
    myPos[0] = startY;
    myPos[1] = startX;
    myPos[2] = 0;
    linesArrayCopy[(int)pathY[i]][(int)pathX[i]]= '#';

    for(int k = 0;; k++)
    {
        var temp = makeMove(myPos, linesArrayCopy, false, pathY, pathX);
        if(temp == -2)
        {
            //znalazl cykla
            cyclesNumber++;
            Console.WriteLine($"Na pozyci: Y= {(int)pathY[i]}, X= {(int)pathX[i]} dla k = {k} dla i = {i}");
        }
        if(temp<0){
            //wyszedl
            Console.WriteLine($"Kroki: {k}");
            break;
        }
    }
    Console.WriteLine($"Koniec iteracji : {i}. Obecny wynik: {cyclesNumber}.");
    //Console.WriteLine($"Startowa pozycja to: Y:{startY}, X:{startX}");
    //Console.WriteLine($"Problematyczna pozycja to : {i}. Y= {(int)pathY[i+1]}, X= {(int)pathX[i+1]}");
}
Console.WriteLine("Koniec proramu lol");

void printMaze(char [][]printedList)
{
    Console.WriteLine("Maze Printin.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
    for(int i=0; i<lines.Length;i++)
    {
        for(int j=0; j<lines[0].Length;j++)
        {
            Console.Write(printedList[i][j]);
        }
        Console.Write('\n');
    }
}

int makeMove(int[] pos, char[][]list, bool fillCords, ArrayList pathY, ArrayList pathX)
{
    //check if out
    int [] newPos = new int[3];
    int direction = pos[2];
    switch(direction)
    {
        case 0 :
            newPos[0]=pos[0]-1; //Y
            newPos[1]=pos[1]; //X
            break;
        case 1 :
            newPos[0]=pos[0]; //Y
            newPos[1]=pos[1]+1; //X
            break;
        case 2 :
            newPos[0]=pos[0]+1; //Y
            newPos[1]=pos[1]; //X
            break;
        case 3 :
            newPos[0]=pos[0]; //Y
            newPos[1]=pos[1]-1; //X
            break;
    }

    // check if left area
    if(newPos[0] < 0 || newPos[0]>= list.Length)
    {
        if(fillCords){
            pathY.Add(pos[0]);
            pathX.Add(pos[1]);
        }
        list[pos[0]][pos[1]] = updateChar(list[pos[0]][pos[1]], direction);
        return -1;
    }
    if(newPos[1] < 0 || newPos[1]>= list[0].Length)
    {
        if(fillCords){
            pathY.Add(pos[0]);
            pathX.Add(pos[1]);
        }
        list[pos[0]][pos[1]] = updateChar(list[pos[0]][pos[1]], direction);
        return -1;
    }

    // if(list[newPos[0]][newPos[1]] == Convert.ToChar(pos[2]+'0')){// ttaj zmiana- getDirection spod boola pod pos[2]
    //     return -2;
    // }

    if(checkDirection(list[newPos[0]][newPos[1]], pos[2]))
    {
        return -2;
     }

   // Console.WriteLine($"newPosY={newPos[0]} and newposX={newPos[1]} and Direction={pos[2]}");
   // Console.WriteLine($"posY={pos[0]} and posX={pos[1]} and Direction={pos[2]}");
    //Console.WriteLine($"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
    //if there is obstacle, make a turn right
    if(list[newPos[0]][newPos[1]] == '#')
    {
        // Turn right
        /// previous direction You dumbo!
        list[pos[0]][pos[1]] = updateChar(list[pos[0]][pos[1]], direction);
        direction = (direction + 1) % 4;
        pos[2]= direction;
        //update turn also, without it it was not possible to detect cycles made only by corners (cycles size 4) ???
        list[pos[0]][pos[1]] = updateChar(list[pos[0]][pos[1]], direction);
        //Console.WriteLine($"Y={newPos[0]} and X={newPos[1]} and Direction={pos[2]}");
        return 0;
    }

    int counter = 0;
    if(list[newPos[0]][newPos[1]] == '.' || list[newPos[0]][newPos[1]] == '^')
    {
        // Turn right
        //list[newPos[0]][newPos[1]] = 'V';
        if(fillCords){
            pathY.Add(pos[0]);
            pathX.Add(pos[1]);
        }
        counter++;
    }
    
    // if((list[newPos[0]][newPos[1]] == '.' || list[newPos[0]][newPos[1]] == '^') && (list[pos[0]][pos[1]] == '.' || list[pos[0]][pos[1]] == '^')){//&& (list[pos[0]][pos[1]] == '.' || list[pos[0]][pos[1]] == '^'
    //     //list[pos[0]][pos[1]] = Convert.ToChar(pos[2]+'0');
    //     if(fillCords){
    //         pathY.Add(pos[0]);
    //         pathX.Add(pos[1]);
    //     }
    // }
   list[pos[0]][pos[1]] = updateChar(list[pos[0]][pos[1]], direction);//Convert.ToChar(pos[2]+'0');
    pos[0]=newPos[0]; //Y
    pos[1]=newPos[1]; //X
    return counter;
}

char updateChar(char previous, int direction)
{
   if(previous > 'p' || previous <='a') // if(previous == '.' || previous == '^')
    {
        int temp = 0;
        if (direction < 0 || direction > 3)
            throw new ArgumentOutOfRangeException(nameof(direction), "Direction must be between 0 and 3.");
        temp |= (1<< direction);  
       return (char)(temp + 'a');
    }
    //return Convert.ToChar(pos[2]+'0');

    int value = previous - 'a';

    if (direction < 0 || direction > 3)
        throw new ArgumentOutOfRangeException(nameof(direction), "Direction must be between 0 and 3.");

    value |= (1 << direction);

    return (char)(value + 'a');
}

bool checkDirection(char fieldToCheck, int direction)
{
    if(fieldToCheck > 'p' || fieldToCheck <='a')
    {
            return false;
    }
    // Odejmij kod ASCII 'a', aby operować na wartościach od 0
    int value = fieldToCheck - 'a';

    // Sprawdź, czy directionX jest w zakresie 0-3
    if (direction < 0 || direction > 3)
        throw new ArgumentOutOfRangeException(nameof(direction), "Direction must be between 0 and 3.");

    // Sprawdź wartość bitu pod direction
    return (value & (1 << direction)) != 0;
}


void TestPath(ArrayList pathY, ArrayList pathX)
{
    int mutationsCount = 0;
    for(int i=0; i<pathY.Count;i++)
    {
        for(int j=0; j<pathY.Count;j++)
        {
            if(pathX[i] == pathX[j] && pathY[i]==pathY[j] && i != j)
            {
                Console.WriteLine($"Mutation found Y:{pathY[i]} X:{pathX[i]}");
                mutationsCount++;
            }
        }
    }
    if(mutationsCount != 0)
    {
        Console.WriteLine($"Too many mutations:{mutationsCount}");
        throw new NotImplementedException();
    }else{
        Console.WriteLine($"Liczba mutacji kontrolowana. OMG.");
        //throw new NotImplementedException();
    }
}