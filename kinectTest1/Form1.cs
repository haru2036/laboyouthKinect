using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Kinect;
using Midi;
namespace kinectTest1
{
    public partial class Form1 : Form
    {
        OutputDevice outputDevice = OutputDevice.InstalledDevices[1];
        KinectSensor kinect;
        static float X, Y, Z;
        public Form1()
        {
            InitializeComponent();
            outputDevice.Open();
            try
            {
                if (KinectSensor.KinectSensors.Count == 0)
                {
                    throw new Exception("Kinectが接続されていません");
                }


                // Kinectインスタンスを取得する
                kinect = KinectSensor.KinectSensors[0];

                // すべてのフレーム更新通知をもらう
                kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);

                // Color,Depth,Skeletonを有効にする
                kinect.SkeletonStream.Enable();

                // Kinectの動作を開始する
                kinect.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        // すべてのデータの更新通知を受け取る
        void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {

            // 骨格位置の表示
            ShowSkeleton(e);
        }

        private void ShowSkeleton(AllFramesReadyEventArgs e)
        {

            // スケルトンフレームを取得する
            SkeletonFrame skeletonFrame = e.OpenSkeletonFrame();
            if (skeletonFrame != null)
            {
                // スケルトンデータを取得する
                Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(skeletonData);

                // プレーヤーごとのスケルトンを描画する
                foreach (var skeleton in skeletonData)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                          // 右手の座標を表示
                        foreach (Joint joint in skeleton.Joints)
                        {
                            if (joint.JointType == JointType.HandRight)
                            {
                                label1.Text = "(" + joint.Position.X.ToString() + "," + joint.Position.Y.ToString() + "," + joint.Position.Z.ToString() + ")";
                                X = joint.Position.X;
                                Y = joint.Position.Y;
                                Z = joint.Position.Z;
                                int vx, vy, vz;
                                vx = Math.Min(127,(int)(63.5*(X+1)));
                                vy = Math.Min(127,(int)(63.5 * (Y + 1)));
                                //vz = Math.Min(127,(int)(63.5 * (Z -3)));
                                int cutoff, type,reso;
                                //type = vz;
                                cutoff = vx;
                                reso = vy;


                                outputDevice.SendControlChange(Channel.Channel1, Midi.Control.ModulationWheel, cutoff);
                                //outputDevice.SendControlChange(Channel.Channel2, Midi.Control.ModulationWheel, type);
                                outputDevice.SendControlChange(Channel.Channel3, Midi.Control.ModulationWheel, reso); 
                            }
                        }


                        

                        }
                    }
                }
            }

      

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            outputDevice.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            outputDevice.SendControlChange(Channel.Channel2, Midi.Control.ModulationWheel, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            outputDevice.SendControlChange(Channel.Channel1, Midi.Control.ModulationWheel, 0);
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            outputDevice.SendControlChange(Channel.Channel3, Midi.Control.ModulationWheel, 0); 
        }


        }
    }
