using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;
using zenith.Core.Adaptations;

namespace zenith.Core.Renderers
{
    public class BearSenseRenderer : IRenderer
    {

        private readonly ICoreClientAPI capi;
        private readonly CreatureAdaptations adaptations;
        private int senseTextureID;


        private MeshData mesh;
        private MeshRef meshRef;

        public BearSenseRenderer(ICoreClientAPI capi, CreatureAdaptations adaptations)
        { 
            this.capi = capi;
            this.adaptations = adaptations;

            //  capi.Logger.Warning($"[RENDERER] GOT {bearSense.GetHashCode()}");
            //  capi.Logger.Warning($"BearSense count: {bearSense?.sensedEntities.Count}");

            senseTextureID = capi.Render.GetOrLoadTexture(new Vintagestory.API.Common.AssetLocation("zenith", "textures/icons/sense.png"));
          
            float x = capi.Render.FrameWidth / 2f;
            float y = capi.Render.FrameHeight / 2f;
            capi.Event.RegisterRenderer(this, EnumRenderStage.Ortho); // NEVER FORGET RENDER STAGE

            CreateMesh();
        }

        public void OnRenderFrame(float dt, EnumRenderStage enumRenderStage)
        {

            //     capi.Logger.Notification($"[RENDERER]  Entities: {bearSense.sensedEntities.Count}");


            var bearSense = adaptations?.BearSenses;

            if (bearSense == null) return;

            capi.Render.OrthoMode(capi.Render.FrameWidth, capi.Render.FrameHeight);
            capi.Render.GlMatrixModeModelView();

            //  int textureId = capi.BlockTextureAtlas.Positions["stone"].atlasTextureId;
            // capi.Logger.Notification($"[RENDERER] BearSense exists. Count: {bearSense.sensedEntities.Count}");

            try
            {


                foreach (var sensed in bearSense.sensedEntities)
                {
                    Vec3d markerPos = sensed.WorldPosition.OffsetCopy(0, 1, 0);

                    Vec3d screenPos = MatrixToolsd.Project(markerPos, capi.Render.PerspectiveProjectionMat,
                    capi.Render.PerspectiveViewMat, capi.Render.FrameWidth,
                    capi.Render.FrameHeight);


                    //if (screenPos.X < 0 || screenPos.X > capi.Render.FrameWidth)
                    //    continue;

                    //if (screenPos.Y < 0 || screenPos.Y > capi.Render.FrameHeight)
                    //    continue;

                    bool behind = screenPos.Z <= 0;
                        

                    float margin = 32;

                    float x = (float)screenPos.X;
                    float rawy = (float)screenPos.Y;
                    float y = capi.Render.FrameHeight - (float)screenPos.Y;

                    //if (behind)
                    //{
                    //    x = capi.Render.FrameWidth/2;

                    //}

                      x = Math.Clamp(x, margin, capi.Render.FrameWidth - margin);

                    y = Math.Clamp(y, margin, capi.Render.FrameHeight - margin);

                    capi.Render.GlPushMatrix();
                //    capi.Render.GlTranslate(x, y, 0);

                   float pulse = 1f + 0.25f * GameMath.Sin((float)capi.ElapsedMilliseconds / 150f);

                   float size = 32 * pulse;

                   



                    
                    
                     //  capi.Render.GlScale(pulse, pulse, 1f);

                        capi.Render.BindTexture2d(senseTextureID);
                      capi.Render.Render2DTexture(senseTextureID, x - size/2, y - size/2, size,size);
                    capi.Logger.Notification(
    $"WorldY: {sensed.WorldPosition.Y} ScreenY: {screenPos.Y}"
);
                    //        capi.Logger.Notification($"Marker: {x}, {y}, Z:{screenPos.Z}");



                    capi.Render.GlPopMatrix();
                    

                }
            }
            finally
            {
                capi.Render.PerspectiveMode();
            }


        }

        private void CreateMesh()
        {
            mesh = new MeshData(4, 6);

            mesh.AddVertex(-50, 50, 0, 0, 1);
            mesh.AddVertex(50, 50, 0, 1, 1);
            mesh.AddVertex(50, -50, 0, 1, 0);
            mesh.AddVertex(-50, -50, 0, 0, 0);

            mesh.AddIndices(0, 1, 2, 0, 2, 3);

            meshRef = capi.Render.UploadMesh(mesh);

        }


        public int RenderRange=> 999;

        public double RenderOrder => 0.1f;

        public void Dispose()
        {
            meshRef?.Dispose();
        }

    }
}
