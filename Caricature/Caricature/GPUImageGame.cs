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
        Effect RenderImage;
        BasicEffect BasicGrid;
        private Buffer<VertexPositionColor> vertices;
        private VertexInputLayout inputLayout;
        VertexPositionColor[] columns;
        //texture which is passed to GPU
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
            InTex = Content.Load<Texture2D>("bigWalt.dds");

            BasicGrid = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f),
                World = Matrix.Identity
            };

            Color vertexColor = Color.White;
            float top = BasicGrid.Projection.M11 / BasicGrid.Projection.M22;
            float edge = 1.0f;

            columns = new VertexPositionColor[84];
            float cur = -1.0f;
            int i = 0;

            //add vertical lines
            for(i = 0; i < columns.Length/2; i ++){
                columns[i] = new VertexPositionColor(new Vector3(cur, top, -1.0f), vertexColor);
                i++;
                columns[i] = new VertexPositionColor(new Vector3(cur, -top, -1.0f), vertexColor);

                cur += edge / 10.0f;
            }
            cur = -top;

            //add horizontal lines
            for (; i < columns.Length; i++)
            {
                columns[i] = new VertexPositionColor(new Vector3(1.0f, cur, -1.0f), vertexColor);
                i++;
                columns[i] = new VertexPositionColor(new Vector3(-1.0f, cur, -1.0f), vertexColor);

                cur += top / 10.0f;
            }

            vertices = SharpDX.Toolkit.Graphics.Buffer.Vertex.New<VertexPositionColor>(GraphicsDevice, columns.Length);
            vertices.SetData(columns);
            
            // Create an input layout from the vertices
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);

            base.LoadContent();
        }

        /// <summary>
        /// Update doesn't do anything. Just here for completeness
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {

            columns[20].Position.X += 0.001f;

            vertices.SetData(columns);

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
            GraphicsDevice.SetVertexBuffer(vertices);
            GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            BasicGrid.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.Draw(PrimitiveType.LineList, vertices.ElementCount);

            base.Draw(gameTime);
        }


    }
}
