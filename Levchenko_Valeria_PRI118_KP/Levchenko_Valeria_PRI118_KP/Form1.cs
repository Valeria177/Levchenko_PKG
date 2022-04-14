using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.FreeGlut;
using Tao.OpenGl;
using System.Threading;
using System.Media;
using System.IO;
using Tao.DevIl;

namespace Levchenko_Valeria_PRI118_KP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private Explosion BOOOOM_1 = new Explosion(1, 10, 1, 300, 100);
        private Explosion BOOOOM_2 = new Explosion(1, 10, 1, 300, 100);

        float global_time = 0;
        double angle = 3, angleX = -96, angleY = 0, angleZ = -30;
        double sizeX = 1, sizeY = 1, sizeZ = 1;

        double translateX = -9, translateY = -60, translateZ = -10;

        double cameraSpeed;

        double chandelierRotete, deltaRotate;
        bool lampBoom;

        float deltaColor;
        int imageId; uint mGlTextureObject;


        double bugTranslateX = 0, bugTranslateY = 0, bugTranslateZ = 0, bugTranslateDelta = 0.2;
        double aimTranslateX = 0, aimTranslateY = 0, aimTranslateZ = 0, aimTranslateDelta = 0.5;
        bool bugLeft = false;
        double strikeRote = 30, deltaStrikeRote = 4;
        bool strike = false;
        bool bugIsAlive = true;

        int currentChannel;
        string[] channels = new string[4] { "../../texture/BestChannel.jpg", "../../texture/channel1.jpg", "../../texture/channel2.jpg", "../../texture/channel3.jpg" };



        private void Form1_Load(object sender, EventArgs e)
        {
            deltaRotate = 1;
            currentChannel = 0;

            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE);
            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);
            Gl.glClearColor(255, 255, 255, 1);
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(60, (float)AnT.Width / (float)AnT.Height, 0.1, 800);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glEnable(Gl.GL_DEPTH_TEST);
     
            Il.ilGenImages(1, out imageId);
            Il.ilBindImage(imageId);
            if (Il.ilLoadImage(channels[currentChannel]))
            {
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                switch (bitspp)
                {
                    case 24:
                        mGlTextureObject = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        mGlTextureObject = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }
            }
            Il.ilDeleteImages(1, ref imageId);

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            cameraSpeed = 5;
            numericUpDown1.Value = 5;
            deltaColor = 0;
            lampBoom = false;
            RenderTimer.Start();
        }


        
        private void timer1_Tick(object sender, EventArgs e)
        {
            global_time += (float)RenderTimer.Interval / 1000;
            Draw();
        }

        private void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glClearColor(255, 255, 255, 1);
            Gl.glLoadIdentity();
            Gl.glPushMatrix();
            Gl.glRotated(angleX, 1, 0, 0); Gl.glRotated(angleY, 0, 1, 0); Gl.glRotated(angleZ, 0, 0, 1);
            Gl.glTranslated(translateX, translateY, translateZ);
            Gl.glScaled(sizeX, sizeY, sizeZ);
            Gl.glColor3f(0.07f, 0.04f, 0.56f);
            BOOOOM_2.Calculate(global_time);
            Gl.glPushMatrix();
            Gl.glRotated(chandelierRotete, 0, 100, 35);
            BOOOOM_1.Calculate(global_time);
            Gl.glPopMatrix();


            Gl.glPushMatrix();
            Gl.glColor3f(0.6431f -deltaColor, 0.4f - deltaColor, 0.1569f - deltaColor);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex3d(200, 200, 35);
            Gl.glVertex3d(-200, 200, 35);
            Gl.glVertex3d(-200, -10, 35);
            Gl.glVertex3d(200, -10, 35);
            Gl.glEnd();
            //пол
            Gl.glColor3f(0.3f - deltaColor, 0.1f - deltaColor, 0f - deltaColor);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex3d(200, 200, 0);
            Gl.glVertex3d(-200, 200, 0);
            Gl.glVertex3d(-200, -10, 0);
            Gl.glVertex3d(200, -10, 0);
            Gl.glEnd();
            double line = 0;
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(3f);
            do
            {
                Gl.glBegin(Gl.GL_LINE_STRIP);
                Gl.glVertex3d(200-line, 200, 0.1);
                Gl.glVertex3d(200-line, -10, 0.1);
                Gl.glEnd();
                line += 5;
            }
            while (line < 400);
            Gl.glPopMatrix();

            //люстра моя прекрасная
            Gl.glPushMatrix();
            Gl.glTranslated(0, 0, -15);
            if (chandelierRotete > 45)
                deltaRotate = -1;
            if (chandelierRotete < -45)
                deltaRotate = 1;
            chandelierRotete += deltaRotate;
            Gl.glRotated(chandelierRotete, 0, 100, 50);
             if (lampBoom)
            {
                BOOOOM_1.SetNewPosition(0, 100, 24);
                BOOOOM_1.SetNewPower(80);
                BOOOOM_1.Boooom(global_time);
                lampBoom = false;
            }
            Gl.glColor3f(0.7529f - deltaColor, 0.7529f - deltaColor, 0.7529f - deltaColor);
            Gl.glLineWidth(4f);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(0, 100, 50);
            Gl.glVertex3d(0, 100, 41);
            Gl.glEnd();
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(0, 100, 45);
            Gl.glVertex3d(-4, 96, 39);
            Gl.glEnd();
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(0, 100, 45);
            Gl.glVertex3d(4, 96, 39);
            Gl.glEnd();
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(0, 100, 45);
            Gl.glVertex3d(4, 104, 39);
            Gl.glEnd();
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(0, 100, 45);
            Gl.glVertex3d(-4, 104, 39);
            Gl.glEnd();

            circle(0, 100, 39, 3, 0.5f);
            Point[] lamp = new Point[40];
            double grad = Math.PI * 2 / 40;
            for (int i = 0; i < 40; i++)
            {
                lamp[i] = new Point();
                lamp[i].x = Math.Cos(grad * i) * 1.2; ;
                lamp[i].y = Math.Sin(grad * i) * 1.2 + 100;
                lamp[i].z = 43;
            }
            Gl.glLineWidth(3f);
            lamp = cilinder(lamp, 0, 100, 42, 1, 0.8784f - deltaColor, 0.8784f - deltaColor, 0.5804f - deltaColor);
            lamp = cilinder(lamp, 0, 100, 40.5, 2, 0.8784f - deltaColor, 0.8784f - deltaColor, 0.5804f - deltaColor);
            lamp = cilinder(lamp, 0, 100, 39, 3, 0.8784f - deltaColor, 0.8784f -deltaColor, 0.5804f - deltaColor);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 0, -1);
            circle(-4, 104, 39, 1, 0.5f);
            for (int i = 0; i < 40; i++)
            {
                lamp[i] = new Point();
                lamp[i].x = Math.Cos(grad * i) * 1 - 4;
                lamp[i].y = Math.Sin(grad * i) * 1 + 104;
                lamp[i].z = 39;
            }
            lamp = cilinder(lamp, -4, 104, 41, 0.6, 0.8784f - deltaColor, 0.8784f - deltaColor, 0.5804f - deltaColor);
            lamp = cilinder(lamp, -4, 104, 42, 0.8, 0.8784f - deltaColor, 0.8784f - deltaColor, 0.5804f - deltaColor);
            Gl.glPopMatrix();
            Gl.glPushMatrix();
            for (int i = 0; i < 40; i++)
            {
                lamp[i] = new Point();
                lamp[i].x = Math.Cos(grad * i) * 1 - 4;
                lamp[i].y = Math.Sin(grad * i) * 1 + 104;
                lamp[i].z = 39;
            }
            Gl.glTranslated(8, 0, -1);
            circle(-4, 104, 39, 1, 0.5f);
            lamp = cilinder(lamp, -4, 104, 41, 0.6, 0.8784f - deltaColor, 0.8784f - deltaColor, 0.5804f - deltaColor);
            lamp = cilinder(lamp, -4, 104, 42, 0.8, 0.8784f - deltaColor, 0.8784f - deltaColor, 0.5804f - deltaColor);
            Gl.glPopMatrix();
            Gl.glPushMatrix();
            for (int i = 0; i < 40; i++)
            {
                lamp[i] = new Point();
                lamp[i].x = Math.Cos(grad * i) * 1 - 4;
                lamp[i].y = Math.Sin(grad * i) * 1 + 104;
                lamp[i].z = 39;
            }
            Gl.glTranslated(8, -8, -1);
            circle(-4, 104, 39, 1, 0.5f);
            lamp = cilinder(lamp, -4, 104, 41, 0.6, 0.8784f - deltaColor, 0.8784f - deltaColor, 0.5804f - deltaColor);
            lamp = cilinder(lamp, -4, 104, 42, 0.8, 0.8784f - deltaColor, 0.8784f - deltaColor, 0.5804f - deltaColor);
            Gl.glPopMatrix();
            Gl.glPushMatrix();
            for (int i = 0; i < 40; i++)
            {
                lamp[i] = new Point();
                lamp[i].x = Math.Cos(grad * i) * 1 - 4;
                lamp[i].y = Math.Sin(grad * i) * 1 + 104;
                lamp[i].z = 39;
            }
            Gl.glTranslated(0, -8, -1);
            circle(-4, 104, 39, 1, 0.5f);
            lamp = cilinder(lamp, -4, 104, 41, 0.6, 0.8784f - deltaColor, 0.8784f - deltaColor, 0.5804f - deltaColor);
            lamp = cilinder(lamp, -4, 104, 42, 0.8, 0.8784f - deltaColor, 0.8784f - deltaColor, 0.5804f - deltaColor);
            Gl.glPopMatrix();

            Gl.glPopMatrix();

            ///телевизор
            Gl.glPushMatrix();
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(-1, 110, 6);
            Gl.glVertex3d(3, 110, 12);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(1.7, 110, 6);
            Gl.glVertex3d(-2.3, 110, 12);
            Gl.glEnd();

            Gl.glTranslated(0, 110, 2);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(4f);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(0, -4, -2);
            Gl.glVertex3d(0, -4, 2);
            Gl.glEnd();

            Gl.glScaled(3, 2, 1);
            Gl.glColor3f(0.7882f-deltaColor, 0.7098f-deltaColor, 0.6078f-deltaColor);
            Glut.glutSolidCube(4);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(4f);
            Glut.glutWireCube(4);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(0.3, 110, 6.05);
            Gl.glScaled(1.4, 1, 1);
            Gl.glColor3f(0.2078f - deltaColor, 0.0392f - deltaColor, 0.0039f - deltaColor);
            Glut.glutSolidCube(4);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(4f);
            Glut.glutWireCube(4);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject);

            Gl.glPushMatrix();
            Gl.glTranslated(-2.1, 107.9, -0.4);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(0, 0, 5);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(4, 0, 5);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(4, 0, 8);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(0, 0, 8);
            Gl.glTexCoord2f(1, 0);
            Gl.glEnd();
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();

            //кнопки телевизора
            double buttonDeltaX = 0;
            double buttonDeltaZ = 0;
            for (int i =0; i < 6; i++)
            {
                for (int j=0; j<2; j++)
                {
                    Gl.glPushMatrix();
                    Gl.glTranslated(2.2+buttonDeltaX, 108.2, 7.43-buttonDeltaZ);
                    Gl.glScaled(0.8, 1, 0.5);
                    Gl.glColor3f(0.7f - deltaColor, 0.7f - deltaColor, 0.7f - deltaColor);
                    Glut.glutSolidCube(0.5);
                    Gl.glColor3f(0, 0, 0);
                    Gl.glLineWidth(1f);
                    Glut.glutWireCube(0.5);
                    Gl.glPopMatrix();
                    buttonDeltaX = 0.5;
                }
                buttonDeltaX = 0;
                buttonDeltaZ += 0.5;
            }

            Gl.glPushMatrix();
            Gl.glTranslated(2.4 + buttonDeltaX, 108.2, 7.43 - buttonDeltaZ);
            Gl.glScaled(1.3, 1, 0.5);
            Gl.glColor3f(0.9765f - deltaColor, 0.1292f - deltaColor, 0);
            Glut.glutSolidCube(0.5);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1f);
            Glut.glutWireCube(0.5);
            Gl.glPopMatrix();


            //Столик
            Gl.glPushMatrix();
            Gl.glTranslated(0, 85, 4);
            Gl.glScaled(1.5, 1, 0.02);
            Gl.glColor3f(0.9804f - deltaColor, 0.5686f - deltaColor, 0);
            Glut.glutSolidCube(10);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Glut.glutWireCube(10);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(7, 80.5, 2);
            Gl.glScaled(0.2, 0.2, 1);
            Gl.glColor3f(0.9804f - deltaColor, 0.5686f - deltaColor, 0);
            Glut.glutSolidCube(4);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Glut.glutWireCube(4);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(-7, 80.5, 2);
            Gl.glScaled(0.2, 0.2, 1);
            Gl.glColor3f(0.9804f - deltaColor, 0.5686f - deltaColor, 0);
            Glut.glutSolidCube(4);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Glut.glutWireCube(4);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(7, 89.5, 2);
            Gl.glScaled(0.2, 0.2, 1);
            Gl.glColor3f(0.9804f - deltaColor, 0.5686f - deltaColor, 0);
            Glut.glutSolidCube(4);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Glut.glutWireCube(4);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(-7, 89.5, 2);
            Gl.glScaled(0.2, 0.2, 1);
            Gl.glColor3f(0.9804f - deltaColor, 0.5686f - deltaColor, 0);
            Glut.glutSolidCube(4);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Glut.glutWireCube(4);
            Gl.glPopMatrix();

            //жучок
            Gl.glPushMatrix();
            Gl.glTranslated(bugTranslateX, bugTranslateY, bugTranslateZ);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 85, 4.1);
            Gl.glScaled(1.8, 1, 0.5);
            Gl.glColor3f(0.6588f - deltaColor, 0 - deltaColor, 0.0588f-deltaColor);
            Glut.glutSolidSphere(0.5, 10,10);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(2f);
            Glut.glutWireSphere(0.5, 10, 5);
            Gl.glPopMatrix();

            if (!bugLeft)
            {
                Gl.glPushMatrix();
                Gl.glTranslated(0.94, 85, 4.2);
                Gl.glScaled(1, 1, 1);
                Gl.glColor3f(0.6588f - deltaColor, 0 - deltaColor, 0.0588f - deltaColor);
                Glut.glutSolidSphere(0.15, 10, 10);
                Gl.glPopMatrix();
            }

            if (bugLeft)
            {
                Gl.glPushMatrix();
                Gl.glTranslated(-0.94, 85, 4.2);
                Gl.glScaled(1, 1, 1);
                Gl.glColor3f(0.6588f - deltaColor, 0 - deltaColor, 0.0588f - deltaColor);
                Glut.glutSolidSphere(0.15, 10, 10);
                Gl.glPopMatrix();
            }


            Gl.glPushMatrix();
            Gl.glTranslated(0.7, 84.7, 4.2);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0.5, -0.25, -0.25);
            Gl.glEnd();
            Gl.glPopMatrix();


            Gl.glPushMatrix();
            Gl.glTranslated(-0.66, 84.7, 4.2);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(-0.5, -0.25, -0.25);
            Gl.glEnd();
            Gl.glPopMatrix();


            Gl.glPushMatrix();
            Gl.glTranslated(0.7, 85.25, 4.2);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0.5, 0.25, -0.25);
            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(-0.66, 85.25, 4.2);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(-0.5, 0.25, -0.25);
            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glPopMatrix();


            //мухобойка
            if (strike)
            {
                Gl.glPushMatrix();
                Gl.glTranslated(0, 85, 4);
                Gl.glTranslated(aimTranslateX-3, aimTranslateY, aimTranslateZ);
                Gl.glRotated(strikeRote, 0, 75, 0);
                strikeRote += deltaStrikeRote;
                if (strikeRote >= 90)
                {
                    BOOOOM_2.SetNewPosition((float)aimTranslateX , 85f+(float)aimTranslateY, 5.5f+(float)aimTranslateZ);
                    BOOOOM_2.SetNewPower(20);
                    BOOOOM_2.Boooom(global_time);
                    strikeRote = 30;
                    strike = false;
                      if (Math.Sqrt((aimTranslateX - bugTranslateX) * (aimTranslateX - bugTranslateX) + (aimTranslateY - bugTranslateY) * (aimTranslateY - bugTranslateY)) < 1.5)
                    {
                        bugIsAlive = false;                 
                    }
                }
                Gl.glColor3f(0.5255f - deltaColor, 0.6824f - deltaColor, 0);
                Glut.glutSolidCylinder(0.2, 2, 10, 10);
                Gl.glLineWidth(2f);
                Gl.glColor3f(0, 0, 0);
                Glut.glutWireCylinder(0.2, 2, 10, 10);
                Gl.glTranslated(0, 0, 3);
                Gl.glColor3f(0, 0.09f - deltaColor, 0.75f - deltaColor);
                Gl.glScaled(0.35, 1, 1);
                Glut.glutSolidSphere(1, 10, 10);
                Gl.glColor3f(0, 0, 0);
                Glut.glutWireSphere(1, 10, 5);
                Gl.glPopMatrix();
            }

            //целеуказатель
            Gl.glPushMatrix();
            Gl.glTranslated(0, 85, 4.2);
            Gl.glTranslated(aimTranslateX, aimTranslateY, aimTranslateZ);
            circle(0, 0, 0, 1, 0.5f);
            Gl.glPopMatrix();

            Gl.glPopMatrix();

            Gl.glFlush();
            AnT.Invalidate();
        }

        private Point[] cilinder(Point[] circle0, double x1, double y1, double z1, double R1, float red, float green, float blue)
        {
            Point[] circle1 = new Point[40];
            int count = 40;
            double grad = Math.PI * 2 / count;
            for (int i = 0; i < count; i++)
            {
                circle1[i] = new Point();
                circle1[i].x = Math.Cos(grad * i) * R1 + x1;
                circle1[i].y = Math.Sin(grad * i) * R1 + y1;
                circle1[i].z = z1;         
            }
            Gl.glColor3f(red, green, blue);
            for (int i = 0; i < count - 1; i++)
            {
                Gl.glColor3f(red, green, blue);
                Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                Gl.glVertex3d(circle0[i].x, circle0[i].y, circle0[i].z);
                Gl.glVertex3d(circle1[i].x, circle1[i].y, circle1[i].z);
                Gl.glVertex3d(circle1[i + 1].x, circle1[i + 1].y, circle1[i + 1].z);
                Gl.glVertex3d(circle0[i + 1].x, circle0[i + 1].y, circle0[i + 1].z);
                Gl.glEnd();
                Gl.glColor3f(0, 0, 0);
                Gl.glBegin(Gl.GL_LINE_STRIP);
                Gl.glVertex3d(circle0[i].x, circle0[i].y, circle0[i].z);
                Gl.glVertex3d(circle0[i + 1].x, circle0[i + 1].y, circle0[i + 1].z);
                Gl.glEnd();
                Gl.glBegin(Gl.GL_LINE_STRIP);
                Gl.glVertex3d(circle1[i].x, circle1[i].y, circle1[i].z);
                Gl.glVertex3d(circle1[i + 1].x, circle1[i + 1].y, circle1[i + 1].z);
                Gl.glEnd();
            }
            Gl.glColor3f(red, green, blue);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex3d(circle0[count - 1].x, circle0[count - 1].y, circle0[count - 1].z);
            Gl.glVertex3d(circle1[count - 1].x, circle1[count - 1].y, circle1[count - 1].z);
            Gl.glVertex3d(circle1[0].x, circle1[0].y, circle1[0].z);
            Gl.glVertex3d(circle0[0].x, circle0[0].y, circle0[0].z);
            Gl.glEnd();
            Gl.glColor3f(0, 0, 0);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(circle0[count - 1].x, circle0[count - 1].y, circle0[count - 1].z);
            Gl.glVertex3d(circle0[0].x, circle0[0].y, circle0[0].z);
            Gl.glEnd();
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(circle1[count - 1].x, circle1[count - 1].y, circle1[count - 1].z);
            Gl.glVertex3d(circle1[0].x, circle1[0].y, circle1[0].z);
            Gl.glEnd();
            return circle1;
        }


        private Point[] circle(double x, double y, double z, double R, float width)
        {
            Point[] krug = new Point[40];
            double grad = 360 / 40;
            double a, b;
            Gl.glLineWidth(width);
            Gl.glColor3f(0, 0, 0);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (int i = 0; i < 40; i++)
            {
                a = -Math.Cos(grad * i) * R + x;
                b = -Math.Sin(grad * i) * R + y;
                krug[i] = new Point(a, b, z);
                Gl.glVertex3d(a, b, z);
            }
            Gl.glEnd();
            return krug;
        }

        private void AnT_KeyDown(object sender, KeyEventArgs e)
        {
            if (comboBox2.SelectedIndex != 1)
            {
                if (e.KeyCode == Keys.W)
                {
                    translateY -= cameraSpeed;

                }
                if (e.KeyCode == Keys.S)
                {
                    translateY += cameraSpeed;
                }
                if (e.KeyCode == Keys.A)
                {
                    translateX += cameraSpeed;
                }
                if (e.KeyCode == Keys.D)
                {
                    translateX -= cameraSpeed;

                }
                if (e.KeyCode == Keys.ControlKey)
                {
                    translateZ += cameraSpeed;

                }
                if (e.KeyCode == Keys.Space)
                {
                    translateZ -= cameraSpeed;
                }
                if (e.KeyCode == Keys.Q)
                {
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            angleX += angle;

                            break;
                        case 1:
                            angleY += angle;

                            break;
                        case 2:
                            angleZ += angle;

                            break;
                        default:
                            break;
                    }
                }
                if (e.KeyCode == Keys.E)
                {
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            angleX -= angle;
                            break;
                        case 1:
                            angleY -= angle;
                            break;
                        case 2:
                            angleZ -= angle;
                            break;
                        default:
                            break;
                    }
                }
                if (e.KeyCode == Keys.Z)
                {
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            sizeX += 0.1;
                            break;
                        case 1:
                            sizeY += 0.1;
                            break;
                        case 2:
                            sizeZ += 0.1;
                            break;
                        default:
                            break;
                    }
                }
                if (e.KeyCode == Keys.X)
                {
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            sizeX -= 0.1;
                            break;
                        case 1:
                            sizeY -= 0.1;
                            break;
                        case 2:
                            sizeZ -= 0.1;
                            break;
                        default:
                            break;
                    }
                }
            }
            if (e.KeyCode == Keys.I && bugTranslateY <= 4.5 && bugIsAlive)
            {
                bugTranslateY += bugTranslateDelta;
            }
            if (e.KeyCode == Keys.K && bugTranslateY >= -4.5 && bugIsAlive)
            {
                bugTranslateY -= bugTranslateDelta;
            }
            if (e.KeyCode == Keys.L && bugTranslateX <= 7 && bugIsAlive)
            {
                bugLeft = false;
                bugTranslateX += bugTranslateDelta;
            }
            if (e.KeyCode == Keys.J && bugTranslateX >= -7 && bugIsAlive)
            {
                bugLeft = true;
                bugTranslateX -= bugTranslateDelta;
            }
            if (e.KeyCode == Keys.T && aimTranslateY <= 4.5)
            {
                aimTranslateY += aimTranslateDelta;
            }
            if (e.KeyCode == Keys.G && aimTranslateY >= -4.5)
            {
                aimTranslateY -= aimTranslateDelta;
            }
            if (e.KeyCode == Keys.H && aimTranslateX <= 7)
            {
                aimTranslateX += aimTranslateDelta;
            }
            if (e.KeyCode == Keys.F && aimTranslateX >= -7)
            {
                aimTranslateX -= aimTranslateDelta;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AnT.Focus();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 0)
                cameraSpeed = (double)numericUpDown1.Value;
            AnT.Focus();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                angle = 3; angleX = -96; angleY = 0; angleZ = -30;
                sizeX = 1; sizeY = 1; sizeZ = 1;
                translateX = -9; translateY = -60; translateZ = -10;
            }
            if (comboBox2.SelectedIndex == 1)
            {
                translateX = 0;
                translateY = -100;
                translateZ = -6;
                angleX = -96;
                angleZ = 0;
            }
            if (comboBox2.SelectedIndex == 2)
            {
                translateX = -7;
                translateY = -74;
                translateZ = -8;
                angleX = -81;
                angleZ = -30;
                numericUpDown1.Value = 1;
            }
            AnT.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            deltaColor = 0.5f;
            lampBoom = true;
            AnT.Focus();
            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            strike = true;
            AnT.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bugIsAlive = true;
            button3.Height = 50;
            button3.Enabled = false;
            button3.Text = "Вы не можете оживить жука более одного раза";
        }

        private void AnT_MouseClick(object sender, MouseEventArgs e)
        {
             if (e.X >= 553 && e.X <= 581 && e.Y >= 419 && e.Y <= 435 && comboBox2.SelectedIndex == 1)
            {
                 currentChannel += 1;
                 if (currentChannel == 4)
                    currentChannel = 0;
                Il.ilGenImages(1, out imageId);
                Il.ilBindImage(imageId);
                if (Il.ilLoadImage(channels[currentChannel]))
                {
                    int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                    int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                    int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                    switch (bitspp)
                    {
                        case 24:
                            mGlTextureObject = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                            break;
                        case 32:
                            mGlTextureObject = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                            break;
                    }
                }
                Il.ilDeleteImages(1, ref imageId);
            }
            if (e.X >= 523 && e.X <= 548 && e.Y >= 419 && e.Y <= 435 && comboBox2.SelectedIndex == 1)
            {
               currentChannel -= 1;
                if (currentChannel == -1)
                    currentChannel = 3;
                Il.ilGenImages(1, out imageId);
                Il.ilBindImage(imageId);
                if (Il.ilLoadImage(channels[currentChannel]))
                {
                    int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                    int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                    int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                    switch (bitspp)
                    {
                        case 24:
                            mGlTextureObject = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                            break;
                        case 32:
                            mGlTextureObject = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                            break;
                    }
                }
                Il.ilDeleteImages(1, ref imageId);
            }
        }
        private static uint MakeGlTexture(int Format, IntPtr pixels, int w, int h)
        {
            uint texObject;
            Gl.glGenTextures(1, out texObject);
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);
            switch (Format)
            {

                case Gl.GL_RGB:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

                case Gl.GL_RGBA:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

            }
            return texObject;
        }
    }
}
