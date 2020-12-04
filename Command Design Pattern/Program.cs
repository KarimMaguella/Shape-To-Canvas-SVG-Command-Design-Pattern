using System;
using System.Collections.Generic;
using System.IO;

namespace commandDesignPattern
{
    //Using the Command Design Pattern approach for this assignment...
    class Program
    {
        // The Canvas is used to manage various Shapes
        // Note - creating this class to hid how it is implemented and provide
        //        add and remove methods (which are just push and pop operations)
        public class Canvas
        {
            // Using a stack to store my Shape objects
            protected int WIDTH = 1450;
            protected int HEIGHT = 850;
            public static Stack<Shape> canvas = new Stack<Shape>();
            public int size { get; }

            public void genFile(String filename)
                {
                String path = @".\OutputFiles\" + filename;

                if (canvas.Count != 0)
                {
                    Console.WriteLine("CREATING XML FILE...");
                    string svgOpen = String.Format(@"<svg height=""{0}"" width=""{1}"" xmlns=""http://www.w3.org/2000/svg"">" + Environment.NewLine, HEIGHT, WIDTH);
                    string svgClose = Environment.NewLine + @"</svg>";
                
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(svgOpen);

                        //This is where you pop a queue of shapes --> from canvas
                        while(canvas.Count != 0)
                        {
                            sw.WriteLine(canvas.Pop().dispSVG + Environment.NewLine);
                        }
                        sw.WriteLine(svgClose);
                    }
                    Console.WriteLine("FILE CREATED!!!");
                }
                else if (!path.Substring(path.Length - 4).ToLower().Equals(".xml"))
                {
                    Console.WriteLine("EXTENSION IS EITHER NOT INCLUDED OR NOT xml");
                }
                else
                {
                    Console.WriteLine("FILE ALREADY EXISTS IN PATH: " + path);
                }
            }
            public void Add(Shape s)
            {

                canvas.Push(s);
                Console.WriteLine("Added Shape to canvas: {0}" + Environment.NewLine, s);
            }
            public Shape Remove()
            {
                Shape s = canvas.Pop();
                Console.WriteLine("Removed Shape from canvas: {0}" + Environment.NewLine, s);
                return s;
            }

            public Canvas()
            {
                size = canvas.Count;
                Console.WriteLine("\nCreated a new Canvas!"); Console.WriteLine();
            }

            public override string ToString()
            {
                String str = "Canvas (" + canvas.Count + " elements): " + Environment.NewLine + Environment.NewLine;
                foreach (Shape s in canvas)
                {
                    str += "   > " + s + Environment.NewLine;
                }
                return str;
            }
        }

        // Abstract Shape class
        public abstract class Shape
        {
            virtual public String dispSVG { get; set; }

            public override string ToString()
            {
                return "Shape!";
            }
        }

        // Square Shape class
        public class Square : Shape
        {
            public int Length { get; private set; }
            public int X { get; private set; }
            public int Y { get; private set; }
            override public String dispSVG { get; set; }

            public Square(int len, int x, int y)
            {
                Length = len;
                X = x;
                Y = y;
                
                dispSVG = String.Format(@"<rect width=""{0}"" height=""{0}"" x=""{1}"" y=""{2}"" stroke=""purple"" stroke-width=""20"" fill=""cyan"" />", Length, X, Y);
            }

            public override string ToString()
            {
                return "Square [x: " + X + ", y: " + Y + ", length: " + Length + "]";
            }
        }

        // Circle Shape class
        public class Circle : Shape
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public int R { get; private set; }
            override public String dispSVG { get; set; }

            public Circle(int x, int y, int r)
            {
                X = x; Y = y; R = r;
                dispSVG = String.Format(@"<circle cx=""{0}"" cy=""{1}"" r=""{2}"" stroke=""black"" stroke-width=""2"" fill=""purple"" />", X, Y, R);
            }

            public override string ToString()
            {
                return "Circle [x: " + X + ", y: " + Y + ", r: " + R + "]";
            }
        }

        public class Rectangle : Shape
        {
            public int X { get; private set;}
            public int Y { get; private set;}
            public int Width { get; private set; }
            public int Height { get; private set; }
            override public String dispSVG { get; set; }

            public Rectangle(int w, int h, int x, int y)
            {
                Width = w;
                Height = h;
                X = x;
                Y = y;
                
                dispSVG = String.Format(@"<rect width=""{0}"" height=""{1}"" x=""{2}"" y=""{3}"" stroke=""purple"" stroke-width=""20"" fill=""cyan"" />", Width, Height, X, Y);
            }

            public override string ToString()
            {
                return "Rectangle [Width: " + Width + ", Height: " + Height + ", X: " + X + ", Y: " + Y + "]";
            }
        }

        // The User (Invoker) Class
        public class User
        {
            private Stack<Command> undo;
            private Stack<Command> redo;

            public int UndoCount { get => undo.Count; }
            public int RedoCount { get => undo.Count; }
            public User()
            {
                Reset();
                Console.WriteLine("Created a new User!"); Console.WriteLine();
            }
            public void Reset()
            {
                undo = new Stack<Command>();
                redo = new Stack<Command>();
            }
            //Concrete Command
            public void Action(Command command)
            {
                // first update the undo - redo stacks
                undo.Push(command);  // save the command to the undo command
                redo.Clear();        // once a new command is issued, the redo stack clears

                // next determine  action from the Command object type
                // this is going to be AddShapeCommand or DeleteShapeCommand
                Type t = command.GetType();
                if (t.Equals(typeof(AddShapeCommand)))
                {
                    Console.WriteLine("Command Received: Add new Shape!" + Environment.NewLine);
                    command.Do();
                }
                if (t.Equals(typeof(DeleteShapeCommand)))
                {
                    Console.WriteLine("Command Received: Delete last Shape!" + Environment.NewLine);
                    command.Do();
                }
                if (t.Equals(typeof(ClearCanvasCommand)))
                {
                    Console.WriteLine("Command Received: Clearing Canvas!" + Environment.NewLine);
                    command.Do();
                }
            }

            // Undo
            public void Undo()
            {
                Console.WriteLine("Undoing operation!"); Console.WriteLine();
                if (undo.Count > 0)
                {
                    Command c = undo.Pop();
                    c.Undo();
                    redo.Push(c);
                }
            }

            // Redo
            public void Redo()
            {
                Console.WriteLine("Redoing operation!"); Console.WriteLine();
                if (redo.Count > 0)
                {
                    Command c = redo.Pop();
                    c.Do();
                    undo.Push(c);
                }
            }
        }

        // Abstract Command class - commands can do something and also undo
        public abstract class Command
        {
            public abstract void Do();     // what happens when we execute (do)
            public abstract void Undo();   // what happens when we unexecute (undo)
        }


        // Add Shape Command - it is a ConcreteCommand Class (extends Command)
        // This adds a Shape (Circle) to the Canvas as the "Do" action
        public class AddShapeCommand : Command
        {
            Shape shape;
            Canvas canvas;

            public AddShapeCommand(Shape s, Canvas c)
            {
                shape = s;
                canvas = c;
            }

            // Adds a shape to the canvas as "Do" action
            public override void Do()
            {
                canvas.Add(shape);
            }
            // Removes a shape from the canvas as "Undo" action
            public override void Undo()
            {
                shape = canvas.Remove();
            }

        }

        // Delete Shape Command - it is a ConcreteCommand Class (extends Command)
        // This deletes a Shape (Circle) from the Canvas as the "Do" action
        public class DeleteShapeCommand : Command
        {
            Shape shape;
            Canvas canvas;

            public DeleteShapeCommand(Canvas c)
            {
                canvas = c;
            }

            // Removes a shape from the canvas as "Do" action
            public override void Do()
            {
                shape = canvas.Remove();
            }

            // Restores a shape to the canvas a an "Undo" action
            public override void Undo()
            {
                canvas.Add(shape);
            }
        }

        public class ClearCanvasCommand : Command
        {
            Stack<Shape> shapes = new Stack<Shape>();
            Canvas canvas;
            int c = 0;
            public ClearCanvasCommand(Canvas c)
            {
                canvas = c;
            }
            public override void Do()
            {
                while(Canvas.canvas.Count != 0)
                {
                    Console.WriteLine(Canvas.canvas.Count);
                    shapes.Push(canvas.Remove());
                    Console.WriteLine("Canvas Cleared!!!");
                    c++;
                }
            }
            public override void Undo()
            {
                while(c != 0)
                {
                    Console.WriteLine();
                    canvas.Add(shapes.Pop());
                    c--;
                }
            }
        }

        /////////// Entry point into application ///////////
        static void Main()
        {
            // This is the start - first seed Random Number Generation (random circle shapes)
            Console.WriteLine(); Console.WriteLine("==== DEMO START ====");
            Random rnd = new Random();

            // Create a Canvas which will hold the list of circles drawn on canvas
            Canvas canvas = new Canvas();
            Console.WriteLine(canvas);

            // Create user and allow user actions (add and delete) circle shapes to a canvas
            User user = new User();

            // User performs actions on the canvas (adding some circles)
            Console.WriteLine("TEST: ADDING (ADD TWO CIRCLES AND ONE RECTANGLE)"); Console.WriteLine();
            user.Action(new AddShapeCommand(new Circle( rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500) ), canvas));
            user.Action(new AddShapeCommand(new Circle( rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500) ), canvas));
            user.Action(new AddShapeCommand(new Rectangle( rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500) ), canvas));
            
            Console.WriteLine(canvas);

            // User performs actions on the canvas (remove some circles)
            Console.WriteLine("TEST: DELETE TWO CIRCLES"); Console.WriteLine();
            user.Action(new DeleteShapeCommand(canvas));
            user.Action(new DeleteShapeCommand(canvas));
            Console.WriteLine(canvas);

            Console.WriteLine("UNDOING: (UN)DELETE TWO (DELETED) CIRCLES"); Console.WriteLine();
            // Undo example commands
            user.Undo(); user.Undo();
            Console.WriteLine(canvas);

            Console.WriteLine("UNDOING: (UN)ADD ONE (ADDED) CIRCLE"); Console.WriteLine();
            // Undo example commands
            user.Undo();
            Console.WriteLine(canvas);


            Console.WriteLine("TEST: ADDING (ADD TWO CIRCLES)"); Console.WriteLine();
            // User performs actions on the canvas (adding some circles)
            user.Action(new AddShapeCommand(new Circle(rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500)), canvas));
            user.Action(new AddShapeCommand(new Circle(rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500)), canvas));
            Console.WriteLine(canvas);

            Console.WriteLine("UNDOING: TWICE"); Console.WriteLine();
            // Undo 3 commands
            user.Undo(); user.Undo();
            Console.WriteLine(canvas);

            Console.WriteLine("REDOING: ONCE"); Console.WriteLine();
            // Redo 2 commands
            user.Redo(); // user.Redo();
            Console.WriteLine(canvas);


            string help = "*********************************************************\n\tCommands:\n\n\tH \t\t Help -displays this message\n\tA <shape>\t Add <shape to canvas>\n\tU\t\t Undo last operation\n \tR \t\t Redo last operation\n \tP \t\t Print Canvas\n \tC \t\t Clear canvas \n \tG <filename> \t Generate Output File in XML\n \tQ \t\t Quit application\n\n\tShapes => [C = Circle, S = Square, R = Rectangle]\n*********************************************************";
            Console.WriteLine(help);

            string input = Console.ReadLine().ToLower();
            while(!input.StartsWith('q'))
            {
                if(input.StartsWith('h'))
                {
                    Console.WriteLine(help);
                }
                else if(input.StartsWith('a'))
                {
                    string[] inputArray = input.Split(' ', StringSplitOptions.TrimEntries);
                    string inputShape = inputArray[1].ToLower();

                    Console.WriteLine(inputShape);
                    //Add Circle By User Input
                    if(inputShape.StartsWith('c'))
                    {
                        user.Action(new AddShapeCommand(new Circle( rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500) ), canvas));
                    }
                    //Add Rectangle By User Input
                    else if(inputShape.StartsWith('r'))
                    {
                        user.Action(new AddShapeCommand(new Rectangle( rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500) ), canvas));
                    }
                    else if(inputShape.StartsWith('s'))
                    {
                        user.Action(new AddShapeCommand(new Square( rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500) ), canvas));
                    }
                }
                else if(input.StartsWith('u'))
                {
                    user.Undo();
                }
                else if(input.StartsWith('r'))
                {
                    user.Redo();
                }
                else if(input.StartsWith('p'))
                {
                    Console.WriteLine(canvas.ToString());
                }
                else if(input.StartsWith('c'))
                {
                    user.Action(new ClearCanvasCommand(canvas));
                }
                else if(input.StartsWith('g'))
                {
                    string[] inputArray = input.Split(' ', StringSplitOptions.TrimEntries);
                    string fileName = inputArray[1].ToLower();

                    canvas.genFile(fileName + ".xml");
                }
                input = Console.ReadLine().ToLower();
            }

            Console.WriteLine();
            Console.WriteLine("Dont't forget to check out any output files you may have created in './OutputFiles' dir");
            Console.WriteLine();

            Console.WriteLine("GoodBye!");
            Console.WriteLine();
        }
    }
}