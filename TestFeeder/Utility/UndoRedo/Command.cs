
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace TestFeeder.UndoRedo
{
    public class Command
    {
        private String name;
        private Object operands;
        private Point position;
        private bool isActive;
        public Object Operands
        {
            get { return operands; }
            set { operands = value; }
        }
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public Command(string Name, Object Operands, Point position=new Point())
        {
            this.name = Name;
            this.operands = Operands;
            this.position = position.CopyObject<Point>();
            this.isActive = true;
        }

        

    }
}
