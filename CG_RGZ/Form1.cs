using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.Serialization;
using SharpGL.SceneGraph.Core;
using SharpGL.Enumerations;
using System.Drawing;
using System.Reflection;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CG_RGZ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private float _rotateX = 0;
        private float _rotateY = 0;
        private float _rotateZ = 0;

        private int _moveX = 0;
        private int _moveY = 0;

        private float _scaleFactor = 1;
        private float _scaleDelta = 1;
        private float _scaleDefault = 1;

        Polygon obj = null;
       
       


        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs e)
        {
            if (obj != null) { 
                OpenGL gl = openGLControl1.OpenGL;

                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
                gl.LoadIdentity();
                gl.LookAt(10, 10, 10, 0, 0, 0, 0, 1, 0);

                gl.Rotate(_rotateX, 1.0f, 0.0f, 0.0f);
                gl.Rotate(_rotateY, 0.0f, 1.0f, 0.0f);
                gl.Rotate(_rotateZ, 0.0f, 0.0f, 1.0f);
            
                gl.Translate(_moveX, 0, 0);
                gl.Translate(0,_moveY,0);
            
                obj.Transformation.ScaleX = _scaleFactor;
                obj.Transformation.ScaleY = _scaleFactor;
                obj.Transformation.ScaleZ = _scaleFactor;
                obj.PushObjectSpace(gl);
                obj.Render(gl, RenderMode.Render);
                obj.PopObjectSpace(gl);
            }
        }
        private void openGLControl1_OpenGLInitialized(object sender, EventArgs e)
        {           
            openGLControl1.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
            
        }
      
     
        int predx = 0;
        int predy = 0;
        private void dmove(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == 's')|| (e.KeyChar == 'ы'))
            {
                _moveY -= 1;
            }
            if ((e.KeyChar == 'a') || (e.KeyChar == 'ф'))
            {
                _moveX -= 1;
            }
            if ((e.KeyChar == 'd') || (e.KeyChar == 'в'))
            {
                _moveX += 1;
            }
            if ((e.KeyChar == 'w') || (e.KeyChar == 'ц'))
            {
                _moveY += 1;
            }
            if ((e.KeyChar == 'z') || (e.KeyChar == 'я'))
            {
                _scaleFactor += (float)0.1;
            }
            if ((e.KeyChar == 'c') || (e.KeyChar == 'с'))
            {
                _scaleFactor -= (float)0.1;
            }
        }
       
        
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {//Обработчик события перемещения мыши по pictuteBox1
            if (e.Button == MouseButtons.Left) //Проверяем нажата ли левая кнопка мыши
            { //запоминаем в point текущее положение курсора мыши
                if ((predx == 0) && (predy == 0))
                {
                    predx = e.X;
                    predy = e.Y;
                }
                else
                {
                    int pointX = e.X;
                    int pointY = e.Y;
                    //if (pointX - predx > 0)
                    //{
                    //    if (pointY - pointY > 0)
                    //        _rotateX += (int)Math.Sqrt(Math.Pow((pointY - predy), 2) + Math.Pow((pointX - predx), 2));
                    //    else _rotateZ += (int)Math.Sqrt(Math.Pow((-pointY + predy), 2) + Math.Pow((-pointX + predx), 2));

                    //}

                    if (pointX-predx<=0)
                    {
                        _rotateY+= pointX - predx;
                    }
                   if (pointX - predx > 0)
                    {
                        _rotateY -= predx-pointX;
                    }
                 
                        if (pointX- predx>0)
                        {
                            _rotateX += pointY - predy;

                        }
                        if (pointX - predx <0)
                        {
                            _rotateZ += pointY - predy;

                       }
                    if (pointX- predx==0)
                    {
                        _rotateX += pointY - predy;
                        _rotateZ += pointY - predy;
                    }
           

                
                    predx = pointX;
                    predy = pointY;
                }
            }
            else
            {
                predx = 0;
                predy = 0;
            }
           


        }
 
      

        private void openGLControl1_Load(object sender, EventArgs e)
        {

        }

        private void mouse_resize(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = ("obj файлы(*.obj) | *.obj");
            if (openDialog.ShowDialog() != DialogResult.OK) return;
            var scene = SerializationEngine.Instance.LoadScene(openDialog.FileName);
            obj = scene.SceneContainer.Children.OfType<Polygon>().FirstOrDefault();

            float[] borders = new float[3];
            obj.BoundingVolume.GetBoundDimensions(out borders[0], out borders[1], out borders[2]);
            float maxBorder = borders.Max();
            float scaleFactor = maxBorder > 10 ? 10.0f / maxBorder : 1;
            obj.Transformation.ScaleX = scaleFactor;
            obj.Transformation.ScaleY = scaleFactor;
            obj.Transformation.ScaleZ = scaleFactor;
            marker = true;
            _scaleFactor = scaleFactor;
            _scaleDefault = scaleFactor;
            _scaleDelta = scaleFactor / 10;
            openGLControl1.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
            openGLControl1.OpenGL.Disable(OpenGL.GL_LIGHTING);
        }
        bool marker = false;
        private void button2_Click(object sender, EventArgs e)
        {
            if (marker == true)
            {
                openGLControl1.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
                openGLControl1.OpenGL.Enable(OpenGL.GL_LIGHTING);
               openGLControl1.OpenGL.Enable(OpenGL.GL_LIGHT0);
            
                openGLControl1.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
            }
        }
    }    
}