using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;

namespace DragDropExample
{
    internal class NodeConnection
    {
        public UIElement StartItem { get; set; }
        public UIElement EndItem { get; set; }
        public Line ConnectionLine { get; set; }
    }
}
