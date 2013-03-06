using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCube
{
    /// <summary>
    /// Simple PointerInput application using SharpDX.Toolkit.
    /// The purpose of this application is to use PointerInput.
    /// </summary>
    /// <remarks>
    /// This application will show a number of most recent pointer events.
    /// Modify the method <see cref="PointerEventDescrption.BuildStringRepresentation"/> to show the data you need.
    /// </remarks>
    public class PointerInputGame : Game
    {
        private class PointerEventDescrption
        {
            private readonly int index;
            private readonly PointerPoint point;

            private readonly string description;

            private string cache;

            public PointerEventDescrption(int index, PointerPoint point)
            {
                this.index = index;
                this.point = point;
                this.description = point.EventType.ToString();
            }

            public PointerPoint Point { get { return point; } }

            public override string ToString()
            {
                if (string.IsNullOrWhiteSpace(cache))
                    BuildStringRepresentation();
                return cache;
            }

            /// <summary>
            /// Modify this method at your taste to display needed information
            /// </summary>
            private void BuildStringRepresentation()
            {
                var sb = new StringBuilder();

                // append general point information
                sb.AppendFormat("{0} - {1}: ", index, description);
                sb.AppendFormat("Dev:{0}; ID:{1}; Pos:{2}; Kind:{3}; ", point.DeviceType, point.PointerId, point.Position, point.PointerUpdateKind);

                // append device-specific information
                switch (point.DeviceType)
                {
                    case PointerDeviceType.Touch:
                        AppendTouchProperties(sb, point);
                        break;
                    case PointerDeviceType.Pen:
                        AppendPenProperties(sb, point);
                        break;
                    case PointerDeviceType.Mouse:
                        AppendMouseProperties(sb, point);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                cache = sb.ToString();
            }

            private void AppendMouseProperties(StringBuilder sb, PointerPoint p)
            {
                sb.AppendFormat("L:{0}; R:{1}; M:{2}; d:{3}", p.IsLeftButtonPressed, p.IsRightButtonPressed, p.IsMiddleButtonPressed, p.MouseWheelDelta);
            }

            private void AppendPenProperties(StringBuilder sb, PointerPoint p)
            {
                sb.AppendFormat("Er:{0}; Rng:{1}; Inv:{2}; Or:{3}; Tw:{4}; Tx:{5}; Ty:{6}", p.IsEraser, p.IsInRange, p.IsInverted, p.Orientation, p.Twist, p.XTilt, p.YTilt);
            }

            private void AppendTouchProperties(StringBuilder sb, PointerPoint p)
            {
                sb.AppendFormat("L:{0}; C:{1}; T:{2}; R:{3}", p.IsLeftButtonPressed, p.IsCanceled, p.TouchConfidence, p.IsInRange);
            }
        }
    }
}
