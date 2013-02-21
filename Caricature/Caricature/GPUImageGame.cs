using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caricature
{
    class GPUImageGame : Game
    {

        GraphicsDeviceManager graphicsDeviceManager;
        /// <summary>
        /// just takes an image, renders it to screen via GPU
        /// </summary>
        Effect RenderImage;
        /// <summary>
        /// draws the grid vertices to screen
        /// </summary>
        BasicEffect BasicGrid;

        /// <summary>
        /// Buffer of vertices
        /// </summary>
        private Buffer<VertexPositionColor> Vertices;
        private VertexInputLayout inputLayout;
        /// <summary>
        /// Array of vertices, this is what we will modify when the user clicks the screen, etc
        /// </summary>
        VertexPositionColor[] m_Vertices;

        /// <summary>
        /// indicies. Maps to elements of vertex buffer
        /// </summary>
        private Buffer<int> Indices;
        /// <summary>
        /// image to draw
        /// </summary>
        Texture2D InTex;

        /// <summary>
        /// basic constructor. Just setting things up - pretty standard
        /// </summary>
        public GPUImageGame()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// This is called once the GraphicsDevice is all loaded up. 
        /// Load any effects, textures, etc
        /// </summary>
        protected override void LoadContent()
        {
            //load the FXO file. MUST BE PRECOMPILED
            RenderImage = Content.Load<Effect>("RenderToScreen.fxo");
            //load image
            InTex = Content.Load<Texture2D>("bigWalt.dds");

            //set up basicGrid effect
            //view = camera pos, projection = frustrum thing, world = ...IDENTITY? D:
            //so top of screen is +/1( BasicGrid.Projection.M11 / BasicGrid.Projection.M22) sides are at +/-1
            BasicGrid = new BasicEffect(GraphicsDevice)
            {
                
                VertexColorEnabled = true,
                View = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f),
                World = Matrix.Identity
            };

            //which color will lines be
            Color vertexColor = Color.Black;
            //view limits
            float top = BasicGrid.Projection.M11 / BasicGrid.Projection.M22;
            float edge = 1.0f;

            //10x10 grid, so 11x11 vertices
            m_Vertices = new VertexPositionColor[121];
            int count = 0;
            //adding vertices
            for (float i = -edge; i <= edge; i += edge / 5.0f)
            {
                for (float j = -top; j <= top; j += top / 5.0f)
                {
                    m_Vertices[count] = new VertexPositionColor(new Vector3(i, j, -1.0f), vertexColor);
                    count++;
                }
            }

            //fuck these things. UGH
            int[] indicies = new int[20*11 *2] {    
    
                0,  1,  1,  2,  2,  3,  3,  4,  4,  5,  5,  6,  6,  7,  7,  8,  8,  9,  9,  10,
                11, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16, 17, 17, 18, 18, 19, 19, 20, 20, 21,
                22, 23, 23, 24, 24, 25, 25, 26, 26, 27, 27, 28, 28, 29, 29, 30, 30, 31, 31, 32,
                33, 34, 34, 35, 35, 36, 36, 37, 37, 38, 38, 39, 39, 40, 40, 41, 41, 42, 42, 43,
                44, 45, 45, 46, 46, 47, 47, 48, 48, 49, 49, 50, 50, 51, 51, 52, 52, 53, 53, 54,
                55, 56, 56, 57, 57, 58, 58, 59, 59, 60, 60, 61, 61, 62, 62, 63, 63, 64, 64, 65,
                66, 67, 67, 68, 68, 69, 69, 70, 70, 71, 71, 72, 72, 73, 73, 74, 74, 75, 75, 76,
                77, 78, 78, 79, 79, 80, 80, 81, 81, 82, 82, 83, 83, 84, 84, 85, 85, 86, 86, 87,
                88, 89, 89, 90, 90, 91, 91, 92, 92, 93, 93, 94, 94, 95, 95, 96, 96, 97, 97, 98,
                99, 100,100,101,101,102,102,103,103,104,104,105,105,106,106,107,107,108,108,109,
                110,111,111,112,112,113,113,114,114,115,115,116,116,117,117,118,118,119,119,120,
                0,  11, 11, 22, 22, 33, 33, 44, 44, 55, 55, 66, 66, 77, 77, 88, 88, 99, 99, 110,
                1,  12, 12, 23, 23, 34, 34, 45, 45, 56, 56, 67, 67, 78, 78, 89, 89, 100,100,111,
                2,  13, 13, 24, 24, 35, 35, 46, 46, 57, 57, 68, 68, 79, 79, 90, 90, 101,101,112,
                3,  14, 14, 25, 25, 36, 36, 47, 47, 58, 58, 69, 69, 80, 80, 91, 91, 102,102,113,
                4,  15, 15, 26, 26, 37, 37, 48, 48, 59, 59, 70, 70, 81, 81, 92, 92, 103,103,114,
                5,  16, 16, 27, 27, 38, 38, 49, 49, 60, 60, 71, 71, 82, 82, 93, 93, 104,104,115,
                6,  17, 17, 28, 28, 39, 39, 50, 50, 61, 61, 72, 72, 83, 83, 94, 94, 105,105,116,
                7,  18, 18, 29, 29, 40, 40, 51, 51, 62, 62, 73, 73, 84, 84, 95, 95, 106,106,117,
                8,  19, 19, 30, 30, 41, 41, 52, 52, 63, 63, 74, 74, 85, 85, 96, 96, 107,107,118,
                9,  20, 20, 31, 31, 42, 42, 53, 53, 64, 64, 75, 75, 86, 86, 97, 97, 108,108,119,
                10, 21, 21, 32, 32, 43, 43, 54, 54, 65, 65, 76, 76, 87, 87, 98, 98, 109,109,120,

            };
            

            //actually create the buffers
            Indices = SharpDX.Toolkit.Graphics.Buffer.Index.New<int>(GraphicsDevice, indicies);
            Vertices = SharpDX.Toolkit.Graphics.Buffer.Vertex.New<VertexPositionColor>(GraphicsDevice, m_Vertices.Length);
            Vertices.SetData(m_Vertices);

            // Create an input layout from the vertices
            inputLayout = VertexInputLayout.FromBuffer(0, Vertices);

            base.LoadContent();
        }

        /// <summary>
        /// Update doesn't do anything. Just here for completeness
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {

            //randomly move a random vertex...in future this will be when the user drags
            //also need to calculate each vertex via FFD formula
            m_Vertices[new Random().Next(m_Vertices.Length - 1)].Position.X += 0.05f * (new Random().Next()%2 == 0?-1:1);

            Vertices.SetData(m_Vertices);
            base.Update(gameTime);
        }

        /// <summary>
        /// where we call the effect
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            //reset the color of the screen...not really important since we are using effects
            GraphicsDevice.Clear(Color.CornflowerBlue);


            // Setup the vertices
            GraphicsDevice.SetVertexBuffer(Vertices);
            GraphicsDevice.SetVertexInputLayout(inputLayout);
            GraphicsDevice.SetIndexBuffer(Indices, true);

            // Apply the basic effect technique and draw the rotating cube
            BasicGrid.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawIndexed(PrimitiveType.LineList, Indices.ElementCount*2);

            base.Draw(gameTime);
        }


    }
}
