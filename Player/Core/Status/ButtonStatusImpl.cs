using NLog;
using System;

namespace Player.Core.Status
{
    class ButtonStatusImpl : ButtonStatus
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private Flags status;

        public override bool Normal
        {
            get { return (status == Flags.Normal); }

            protected set
            {
                if (value)
                    status = Flags.Normal;
            }
        }

        public override bool Selected
        {
            get { return status.HasFlag(Flags.Selected); }

            protected set
            {
                if (value)
                    status |= Flags.Selected;
                else
                    status &= ~Flags.Selected;
            }
        }

        public override bool MouseOver
        {
            get { return status.HasFlag(Flags.MouseOver); }
            
            protected set
            {
                if (value)
                    status |= Flags.MouseOver;
                else
                    status &= ~Flags.MouseOver;
            }
        }

        public override bool Touched
        {
            get { return status.HasFlag(Flags.Touched); }

            protected set
            {
                if (value)
                    status |= Flags.Touched;
                else
                    status &= ~Flags.Touched;
            }
        }

        public void Reset()
        {
            Normal = true;
        }

        public void Update(Flags flag, bool state)
        {
            switch (flag)
            {
                case Flags.Normal:
                    Normal = state;
                    break;
                case Flags.Selected:
                    Selected = state;
                    break;
                case Flags.MouseOver:
                    MouseOver = state;
                    break;
                case Flags.Touched:
                    Touched = state;
                    break;
                default:
                    logger.Warn("Undefined button status flag ({0}) was given! ", flag);
                    break;
            }
        }
    }
}
