// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using SharpDX;
using SharpDX.Toolkit;

namespace MiniCube
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    using System.Collections.Generic;

    /// <summary>
    /// Simple MiniCube application using SharpDX.Toolkit.
    /// The purpose of this application is to show a rotating cube using <see cref="BasicEffect"/>.
    /// </summary>
    public class MiniCubeGame : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private BasicEffect basicEffect;
        private BasicEffect basicEffectNoTex;
        private Buffer<VertexPositionColorTexture> vertices;
        private VertexInputLayout inputLayout;
        VertexPositionColorTexture[] vert;

        private readonly IPointerService pointerService;
        private readonly PointerState pointerState = new PointerState();
        private List<int> CurVertices;

        RenderTarget2D ImageRT, GridRT;

        /// <summary>
        /// Initializes a new instance of the <see cref="MiniCubeGame" /> class.
        /// </summary>
        public MiniCubeGame()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            pointerService = new PointerManager(this);

            CurVertices = new List<int>();

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";
        }


        protected override void LoadContent()
        {

            float Edge = 468;
            float Top = 800;

            int Rows = 3;
            int Cols = 3;

            float ColHeight = Top / (float)Cols;
            float RowWidth = Edge / (float)Rows;

            // Creates a basic effect
            basicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice)
                {
                    VertexColorEnabled = true,
                    View = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY),
                    Projection = Matrix.OrthoOffCenterLH(0, Edge, Top, 0, 0.1f, 100.0f),
                    World = Matrix.Identity
                });

            basicEffect.TextureEnabled = true;
            basicEffect.Texture = Content.Load<Texture2D>("birds.dds");

            // Creates a basic effect
            basicEffectNoTex = ToDisposeContent(new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.OrthoOffCenterLH(0, Edge, Top, 0, 0.1f, 100.0f),
                World = Matrix.Identity,
                TextureEnabled = false
            });

            vert = new VertexPositionColorTexture[6 * Rows * Cols];

            int index = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    vert[index++] = new VertexPositionColorTexture(new Vector3(RowWidth * i, ColHeight * j, 1.0f), Color.White, new Vector2((float)i / (float)Rows, (float)j / (float)Cols)); // Top Left
                    vert[index++] = new VertexPositionColorTexture(new Vector3((i + 1) * RowWidth, (j + 1) * ColHeight, 1.0f), Color.White, new Vector2((float)i / (float)Rows + 1.0f / (float)Rows, (float)j / (float)Cols + 1.0f / (float)Cols)); // Bottom Right
                    vert[index++] = new VertexPositionColorTexture(new Vector3(RowWidth * i, (j + 1) * ColHeight, 1.0f), Color.White, new Vector2((float)i / (float)Rows, (float)j / (float)Cols + 1.0f / (float)Cols)); // Bottom Left

                    vert[index++] = new VertexPositionColorTexture(new Vector3(RowWidth * i, ColHeight * j, 1.0f), Color.White, new Vector2((float)i / (float)Rows, (float)j / (float)Cols)); // Top Left
                    vert[index++] = new VertexPositionColorTexture(new Vector3((i + 1) * RowWidth, ColHeight * j, 1.0f), Color.White, new Vector2((float)i / (float)Rows + 1.0f / (float)Rows, (float)j / (float)Cols)); //Top Right
                    vert[index++] = new VertexPositionColorTexture(new Vector3((i + 1) * RowWidth, (j + 1) * ColHeight, 1.0f), Color.White, new Vector2((float)i / (float)Rows + 1.0f / (float)Rows, (float)j / (float)Cols + 1.0f / (float)Cols)); // Bottom Right

                }
            }

            // Creates vertices for the cube
            vertices = ToDisposeContent(Buffer.Vertex.New<VertexPositionColorTexture>(
                GraphicsDevice, vert.Length));
            ToDisposeContent(vertices);

            // Create an input layout from the vertices
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "MiniCube demo";

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // Rotate the cube.
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
           // basicEffect.World = Matrix.RotationX(time) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f);
            float buff = 20.0f;


            vertices.SetData(vert);
            
            pointerService.GetState(pointerState);

            try
            {
                foreach (PointerPoint point in pointerState.Points)
                {

                    if (point.EventType == PointerEventType.Pressed)
                    {
                        if (CurVertices.Count == 0)
                        {
                            for (int i = 0; i < vert.Length; i++)
                            {
                                if (vert[i].Position.X > point.Position.X - buff && vert[i].Position.X < point.Position.X + buff &&
                                    vert[i].Position.Y > point.Position.Y - buff && vert[i].Position.Y < point.Position.Y + buff)
                                {
                                    CurVertices.Add(i);
                                }
                            }
                        }
                    }
                    else if (point.EventType == PointerEventType.Released)
                    {
                        CurVertices.Clear();
                    }
                    else if (point.EventType == PointerEventType.Moved)
                    {
                        if (CurVertices.Count > 0)
                            foreach (int i in CurVertices)
                            {
                                vert[i].Position.X = point.Position.X;
                                vert[i].Position.Y = point.Position.Y;
                            }
                    }

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Setup the vertices
            GraphicsDevice.SetVertexBuffer(vertices);
            GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            basicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);

            basicEffectNoTex.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.Draw(PrimitiveType.LineList, vertices.ElementCount);


            // Handle base.Draw
            base.Draw(gameTime);
        }

    }
}
