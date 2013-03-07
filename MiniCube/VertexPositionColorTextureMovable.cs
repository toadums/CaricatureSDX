using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCube
{
    struct VertexPositionColorTextureMovable
    {
        private VertexPositionColorTexture m_Vertex;
        private bool m_Movable;

        public VertexPositionColorTexture Vertex
        {
            get { return m_Vertex; }
            set { m_Vertex = value; }
        }

        public bool Movable
        {
            get { return m_Movable; }
            set { m_Movable = value; }
        }

        public Vector3 Position
        {
            get { return m_Vertex.Position; }
            set { m_Vertex.Position = value; }
        }

        public Vector2 TextureCoordinate
        {
            get { return m_Vertex.TextureCoordinate; }
            set { m_Vertex.TextureCoordinate = value; }
        }

        public Color Color
        {
            get { return m_Vertex.Color; }
            set { m_Vertex.Color = value; }
        }

        public VertexPositionColorTextureMovable(VertexPositionColorTexture v, bool m)
        {
            m_Vertex = v;
            m_Movable = m;
        }

        public VertexPositionColorTextureMovable(Vector3 pos, Color col, Vector2 tex, bool m)
        {
            m_Vertex = new VertexPositionColorTexture(pos, col, tex);
            m_Movable = m;
        }

        

    }
}
