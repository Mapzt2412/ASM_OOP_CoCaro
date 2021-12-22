using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoCaro
{
    public class Player
    {
        #region Properties
        private string name;

        public string Name { get => name; set => name = value; }       

        private Image mark;

        public Image Mark { get => mark; set => mark = value; }
        #endregion

        #region Initialize
        public Player(string name, Image mark)
        {
            this.Name = name;
            this.Mark = mark;
        }
        #endregion

        #region Methods          
        #endregion
    }
}
