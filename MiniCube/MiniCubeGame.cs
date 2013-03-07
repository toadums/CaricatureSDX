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

        #region effects
        Effect ToonFXCMY64AutoConstrastColorFilter;
        Effect ToonFXCMY64ColorFilter;
        Effect ToonFXFlowBilateralLineFilter;
        Effect ToonFXFlowDogFilter;
        Effect ToonFXFlowFromStructureTensor;
        Effect ToonFXGaussianFilter;
        Effect ToonFXLineIntegralConvolutionFilter;
        Effect ToonFXPrepareForDogFilter;
        Effect ToonFXQuantizeColorsUsingFlowFilter;
        Effect ToonFXStructureTensorUsingSobelFilter;
        Effect ToonFXThresholdDogFilter;
        Effect RenderToScreen;
        Effect GPUImageMultiplyBlendFilter;
        #endregion

        /// <summary>
        /// The image we want to toon
        /// </summary>
        public static Texture2D CurrentImage;
        /// <summary>
        /// Our RenderTargets to use as output/input to shaders
        /// </summary>
        private RenderTarget2D RenderTarget0, RenderTarget1, RenderTarget2, RenderTarget3, RenderTarget4, RenderTarget5;

        /// <summary>
        /// I think texeloffset determines where in the texture you start at (ie. 'changes' 0,0)
        /// </summary>
        private float texelOffset = 0.0012f;

        /// <summary>
        /// Curviness
        /// </summary>
        public float sigmaFlow = 2.66f;

        /// <summary>
        /// Edge width
        /// </summary>
        public float sigmaDog = 0.9f + 0.9f * 1.0f;

        /// <summary>
        /// Not controlled by user
        /// </summary>
        private float licSigma = 4.97f;

        /// <summary>
        /// Not controlled by user
        /// </summary>
        private float sigma_s = 5.4f;

        /// <summary>
        /// Not controlled by user
        /// </summary>
        private float sigma_t = 8.0f;

        /// <summary>
        /// Not controlled by user
        /// </summary>
        private float aniso = 2.0f;

        /// <summary>
        /// number of hue bins, controls how much effect the hue has on the computer color
        /// </summary>
        public float hueBins = 24.0f;

        /// <summary>
        /// number of sat bins, controls how much effect the sat has on the computer color
        /// </summary>             
        public float satBins = 24.0f;

        /// <summary>
        /// number of val bins, controls how much effect the val has on the computer color
        /// </summary>     
        public float valBins = 8.0f;

        /// <summary>
        /// Stroke
        /// </summary>
        public float edge_offset = 0.07f;

        /// <summary>
        /// Amount of gray
        /// </summary>        
        public float grey_offset = 2.5f;

        /// <summary>
        /// amount of black
        /// </summary>     
        public float black_offset = 2.65f;

        /// <summary>
        /// Size of the render target, so it only needs to be set once
        /// </summary>
        private Vector2 ImageSize;

        private bool BeenRendered = false;

        private GraphicsDeviceManager graphicsDeviceManager;
        private BasicEffect basicEffect;
        private BasicEffect basicEffectNoTex;
        private Buffer<VertexPositionColorTexture> vertices;
        private VertexInputLayout inputLayout;
        VertexPositionColorTexture[] vert;


        private readonly IPointerService pointerService;
        private readonly PointerState pointerState = new PointerState();
        private List<int> CurVertices;

        private Effect DrawPoints;

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

        private void LoadEffectAndRT()
        {
            
                //Load all the effects
                ToonFXCMY64AutoConstrastColorFilter = ToDisposeContent(Content.Load<Effect>(@"HLSL\ToonFXCMY64AutoConstrastColorFilter.fxo"));
                ToonFXCMY64ColorFilter = ToDisposeContent(Content.Load<Effect>(@"HLSL\ToonFXCMY64ColorFilter.fxo"));
                ToonFXFlowBilateralLineFilter = ToDisposeContent(Content.Load<Effect>(@"HLSL\ToonFXFlowBilateralLineFilter.fxo"));
                ToonFXFlowDogFilter = ToDisposeContent(Content.Load<Effect>(@"HLSL\ToonFXFlowDogFilter.fxo"));
                ToonFXFlowFromStructureTensor = ToDisposeContent(Content.Load<Effect>(@"HLSL\ToonFXFlowFromStructureTensor.fxo"));
                ToonFXGaussianFilter = ToDisposeContent(Content.Load<Effect>(@"HLSL\ToonFXGaussianFilter.fxo"));
                ToonFXLineIntegralConvolutionFilter = ToDisposeContent(Content.Load<Effect>(@"HLSL\ToonFXLineIntegralConvolutionFilter.fxo"));
                ToonFXPrepareForDogFilter = ToDisposeContent(Content.Load<Effect>(@"HLSL\ToonFXPrepareForDogFilter.fxo"));
                ToonFXQuantizeColorsUsingFlowFilter = ToDisposeContent(Content.Load<Effect>(@"HLSL\ToonFXQuantizeColorsUsingFlowFilter.fxo"));
                ToonFXStructureTensorUsingSobelFilter = ToDisposeContent(Content.Load<Effect>(@"HLSL\ToonFXStructureTensorUsingSobelFilter.fxo"));
                ToonFXThresholdDogFilter = ToDisposeContent(Content.Load<Effect>(@"HLSL\ToonFXThresholdDogFilter.fxo"));
                RenderToScreen = ToDisposeContent(Content.Load<Effect>(@"HLSL\RenderToScreen.fxo"));
                GPUImageMultiplyBlendFilter = ToDisposeContent(Content.Load<Effect>(@"HLSL\GPUImageMultiplyBlendFilter.fxo"));

                //load all the render targets
                RenderTarget0 = ToDisposeContent(RenderTarget2D.New(GraphicsDevice, GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height, PixelFormat.B8G8R8A8.UNorm));
                RenderTarget1 = ToDisposeContent(RenderTarget2D.New(GraphicsDevice, GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height, PixelFormat.B8G8R8A8.UNorm));
                RenderTarget2 = ToDisposeContent(RenderTarget2D.New(GraphicsDevice, GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height, PixelFormat.B8G8R8A8.UNorm));
                RenderTarget3 = ToDisposeContent(RenderTarget2D.New(GraphicsDevice, GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height, PixelFormat.B8G8R8A8.UNorm));
                RenderTarget4 = ToDisposeContent(RenderTarget2D.New(GraphicsDevice, GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height, PixelFormat.B8G8R8A8.UNorm));
                RenderTarget5 = ToDisposeContent(RenderTarget2D.New(GraphicsDevice, GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height, PixelFormat.B8G8R8A8.UNorm));

                ImageSize = new Vector2(GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height);
        }

        protected override void LoadContent()
        {

            LoadEffectAndRT();

            float Edge = 468;
            float Top = 800;

            int Rows = 10;
            int Cols = 10;

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
            CurrentImage = Content.Load<Texture2D>("BigWalt.dds");

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

            Color VertexColor = Color.Red;

            int index = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    
                    vert[index++] = new VertexPositionColorTexture(new Vector3((i + 1) * RowWidth, (j + 1) * ColHeight, 1.0f), Color.White, new Vector2((float)i / (float)Rows + 1.0f / (float)Rows, (float)j / (float)Cols + 1.0f / (float)Cols)); // Bottom Right
                    vert[index++] = new VertexPositionColorTexture(new Vector3(RowWidth * i, (j + 1) * ColHeight, 1.0f), Color.White, new Vector2((float)i / (float)Rows, (float)j / (float)Cols + 1.0f / (float)Cols)); // Bottom Left
                    vert[index++] = new VertexPositionColorTexture(new Vector3(RowWidth * i, ColHeight * j, 1.0f), Color.White, new Vector2((float)i / (float)Rows, (float)j / (float)Cols)); // Top Left

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
            float buff = 10.0f;

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

            if (!BeenRendered)
            {

                BandWhite(CurrentImage);

                BeenRendered = true;
            }

            basicEffect.Texture = RenderTarget4;

            GraphicsDevice.SetRenderTargets(GraphicsDevice.BackBuffer);


            // Setup the vertices
            GraphicsDevice.SetVertexBuffer(vertices);
            GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            basicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);

            for (int i = 0; i < vert.Length; i++)
            {
                vert[i].Color = Color.Red;
            }

            vertices.SetData(vert);

            basicEffectNoTex.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.Draw(PrimitiveType.LineList, vertices.ElementCount);

            for (int i = 0; i < vert.Length; i++)
            {
                vert[i].Color = Color.White;
            }

            vertices.SetData(vert);

            // Handle base.Draw
            base.Draw(gameTime);
        }

        /// <summary>
        /// Just compute the black lines of the image
        /// </summary>
        /// <param name="tex">Image to print</param>
        /// <returns>The renderTarget which holds the tooned picture</returns>
        private RenderTarget2D BandWhite(Texture2D tex)
        {

            GraphicsDevice.SetRenderTargets(RenderTarget5);
            RenderToScreen.Parameters["InputTexture"].SetResource(tex);
            RenderToScreen.Parameters["Orientation"].SetValue(0);
            GraphicsDevice.DrawQuad(RenderToScreen);

            GetThreshold(RenderTarget5);

            return RenderTarget4;

        }

        /// <summary>
        /// Apply the filters to get the outline of the image
        /// </summary>
        /// <param name="tex">Image to print</param>
        private void GetThreshold(RenderTarget2D tex)
        {
            StructureTensor(tex);
            TensorSmoothing(RenderTarget2);
            Flow(RenderTarget3);
            BilateralLine(tex, RenderTarget2, sigma_s, sigma_t, aniso);
            PrepareForDOG(RenderTarget3);
            DOG(RenderTarget1, RenderTarget2);
            LIC(RenderTarget4, RenderTarget2);
            Threshold(RenderTarget1);
        }

        #region TOONING

        private void StructureTensor(RenderTarget2D rt)
        {
            GraphicsDevice.SetRenderTargets(RenderTarget2);
            ToonFXStructureTensorUsingSobelFilter.Parameters["InputTexture"].SetResource(rt);
            ToonFXStructureTensorUsingSobelFilter.Parameters["ImageSize"].SetValue(ImageSize);
            GraphicsDevice.DrawQuad(ToonFXStructureTensorUsingSobelFilter);

        }

        private void TensorSmoothing(RenderTarget2D rt)
        {
            GraphicsDevice.SetRenderTargets(RenderTarget1);
            ToonFXGaussianFilter.Parameters["InputTexture"].SetResource(rt);
            ToonFXGaussianFilter.Parameters["texelWidthOffset"].SetValue(texelOffset);
            ToonFXGaussianFilter.Parameters["texelHeightOffset"].SetValue(texelOffset);
            ToonFXGaussianFilter.Parameters["sigma_flow"].SetValue(sigmaFlow);
            ToonFXGaussianFilter.Parameters["dir"].SetValue(false);
            GraphicsDevice.DrawQuad(ToonFXGaussianFilter);

            GraphicsDevice.SetRenderTargets(RenderTarget3);
            ToonFXGaussianFilter.Parameters["InputTexture"].SetResource(RenderTarget1);
            ToonFXGaussianFilter.Parameters["dir"].SetValue(true);
            GraphicsDevice.DrawQuad(ToonFXGaussianFilter);
        }

        private void Flow(RenderTarget2D rt)
        {
            GraphicsDevice.SetRenderTargets(RenderTarget2);
            ToonFXFlowFromStructureTensor.Parameters["InputTexture"].SetResource(rt);
            GraphicsDevice.DrawQuad(ToonFXFlowFromStructureTensor);
        }

        private void BilateralLine(RenderTarget2D rt1, RenderTarget2D rt2, float sigmaS, float sigmaT, float aniso)
        {

            GraphicsDevice.SetRenderTargets(RenderTarget1);
            ToonFXFlowBilateralLineFilter.Parameters["InputTexture1"].SetResource(rt1);
            ToonFXFlowBilateralLineFilter.Parameters["InputTexture2"].SetResource(rt2);
            ToonFXFlowBilateralLineFilter.Parameters["texelWidthOffset"].SetValue(texelOffset);
            ToonFXFlowBilateralLineFilter.Parameters["texelHeightOffset"].SetValue(texelOffset);
            ToonFXFlowBilateralLineFilter.Parameters["sigma_s"].SetValue(sigmaS);
            ToonFXFlowBilateralLineFilter.Parameters["sigma_t"].SetValue(sigmaT);
            ToonFXFlowBilateralLineFilter.Parameters["dir"].SetValue(true);
            GraphicsDevice.DrawQuad(ToonFXFlowBilateralLineFilter);

            GraphicsDevice.SetRenderTargets(RenderTarget3);
            ToonFXFlowBilateralLineFilter.Parameters["InputTexture1"].SetResource(RenderTarget1);
            ToonFXFlowBilateralLineFilter.Parameters["InputTexture2"].SetResource(rt2);
            ToonFXFlowBilateralLineFilter.Parameters["sigma_s"].SetValue(sigmaS * aniso);//second bilateral needs only sigma s set, since sigma t will be the same as above
            ToonFXFlowBilateralLineFilter.Parameters["dir"].SetValue(false);
            GraphicsDevice.DrawQuad(ToonFXFlowBilateralLineFilter);

        }

        private void PrepareForDOG(RenderTarget2D rt)
        {
            GraphicsDevice.SetRenderTargets(RenderTarget1);
            ToonFXPrepareForDogFilter.Parameters["InputTexture"].SetResource(rt);
            GraphicsDevice.DrawQuad(ToonFXPrepareForDogFilter);

        }

        private void DOG(RenderTarget2D rt1, RenderTarget2D rt2)
        {
            GraphicsDevice.SetRenderTargets(RenderTarget4);
            ToonFXFlowDogFilter.Parameters["InputTexture1"].SetResource(rt1);
            ToonFXFlowDogFilter.Parameters["InputTexture2"].SetResource(rt2);
            ToonFXFlowDogFilter.Parameters["texelWidthOffset"].SetValue(texelOffset);
            ToonFXFlowDogFilter.Parameters["texelHeightOffset"].SetValue(texelOffset);
            ToonFXFlowDogFilter.Parameters["sigma_dog"].SetValue(sigmaDog);
            GraphicsDevice.DrawQuad(ToonFXFlowDogFilter);
        }

        private void LIC(RenderTarget2D rt1, RenderTarget2D rt2)
        {
            GraphicsDevice.SetRenderTargets(RenderTarget1);
            ToonFXLineIntegralConvolutionFilter.Parameters["InputTexture1"].SetResource(rt1);
            ToonFXLineIntegralConvolutionFilter.Parameters["InputTexture2"].SetResource(rt2);
            ToonFXLineIntegralConvolutionFilter.Parameters["texelWidthOffset"].SetValue(texelOffset);
            ToonFXLineIntegralConvolutionFilter.Parameters["texelHeightOffset"].SetValue(texelOffset);
            ToonFXLineIntegralConvolutionFilter.Parameters["sigma_c"].SetValue(licSigma);
            GraphicsDevice.DrawQuad(ToonFXLineIntegralConvolutionFilter);
        }

        private void Threshold(RenderTarget2D rt)
        {
            GraphicsDevice.SetRenderTargets(RenderTarget4);
            ToonFXThresholdDogFilter.Parameters["InputTexture"].SetResource(RenderTarget1);
            ToonFXThresholdDogFilter.Parameters["edge_offset"].SetValue(edge_offset);
            ToonFXThresholdDogFilter.Parameters["grey_offset"].SetValue(grey_offset);
            ToonFXThresholdDogFilter.Parameters["black_offset"].SetValue(black_offset);
            ToonFXThresholdDogFilter.Parameters["Rainbow"].SetValue(false);
            GraphicsDevice.DrawQuad(ToonFXThresholdDogFilter);
        }

        private void Blend(RenderTarget2D rt1, RenderTarget2D rt2)
        {
            GraphicsDevice.SetRenderTargets(RenderTarget5);
            GPUImageMultiplyBlendFilter.Parameters["InputTexture1"].SetResource(rt1);
            GPUImageMultiplyBlendFilter.Parameters["InputTexture2"].SetResource(rt2);
            GraphicsDevice.DrawQuad(GPUImageMultiplyBlendFilter);
        }

        private void Quantize(RenderTarget2D rt1, RenderTarget2D rt2)
        {
            GraphicsDevice.SetRenderTargets(RenderTarget1);
            ToonFXQuantizeColorsUsingFlowFilter.Parameters["InputTexture1"].SetResource(rt1);
            ToonFXQuantizeColorsUsingFlowFilter.Parameters["InputTexture2"].SetResource(rt2);
            ToonFXQuantizeColorsUsingFlowFilter.Parameters["ValueBins"].SetValue(valBins);
            ToonFXQuantizeColorsUsingFlowFilter.Parameters["SaturationBins"].SetValue(satBins);
            ToonFXQuantizeColorsUsingFlowFilter.Parameters["HueBins"].SetValue(hueBins);
            GraphicsDevice.DrawQuad(ToonFXQuantizeColorsUsingFlowFilter);
        }

        private void CMYToon64(RenderTarget2D rt1)
        {
            GraphicsDevice.SetRenderTargets(RenderTarget3);
            ToonFXCMY64AutoConstrastColorFilter.Parameters["InputTexture"].SetResource(rt1);
            GraphicsDevice.DrawQuad(ToonFXCMY64AutoConstrastColorFilter);
        }
        #endregion

    }
}
