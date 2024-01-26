using System;
using System.Drawing;
using System.Windows.Forms;
using MultilevelLibrary;
using MultilevelLibrary.Drawing;
using CamerasChasingData;
using CamerasChasingData.TemplateManagement;

namespace MultilevelViewer
{
    public partial class Form1 : Form
    {
        private GeneratorMain generator; //генератор лабиринтов
        private MultilevelMaze maze; //многоэтажный лабиринт
        private Template[] templates; //список режимов игры
        private int currentTemplateIndex; //номер текущего режима игры

        public Form1()
        {
            InitializeComponent();
            generator = new GeneratorMain();

            //загрузить режимы игры
            FileTemplateStorage templateStorage = new FileTemplateStorage();
            templates = templateStorage.LoadTemplates("TemplatesDataQuest.txt");

            //создать гиперссылки для них
            int currentLocationY = 16;
            for (int i = 0; i < templates.Length; i++)
            {
                LinkLabel linkTemplate = new LinkLabel
                {
                    Location = new Point(6, currentLocationY),
                    Text = templates[i].Id,
                    AutoSize = true
                };
                linkTemplate.Links.Add(0, linkTemplate.Text.Length, i);
                linkTemplate.LinkClicked += new LinkLabelLinkClickedEventHandler(linkTemplate_LinkClicked);
                groupBox2.Controls.Add(linkTemplate);

                currentLocationY += 13;
                if (i == 11)
                    currentLocationY += 13;
            }

            //отобразить особняк с настройками по умолчанию
            currentTemplateIndex = 2; //Industrial mansion
            TemplateIntoInput();
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            //сгенерировать особняк, соответствующий условиям
            Generate:
            try
            {
                Generate();
            }
            catch (GenerateException ex)
            {
                if (!checkBoxFixed.Checked)
                    goto Generate;
                else
                    MessageBox.Show(ex.Message, "Ошибка генерации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ConditionException ex)
            {
                if (!checkBoxFixed.Checked)
                    goto Generate;
                else
                    MessageBox.Show(ex.Message, "Ошибка проверки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DrawMansion(sender, e);
            buttonSaveModel.Enabled = true;
            buttonSaveImage.Enabled = true;
            buttonSaveAtlas.Enabled = true;
        }

        //сгенерировать особняк
        private void Generate()
        {
            //получить сид
            int seed = checkBoxFixed.Checked ? (int)numericSeed.Value : Environment.TickCount;
            numericSeed.Value = seed;

            //создать новое помещение
            maze = new MultilevelMaze((int)numericWidth.Value, (int)numericHeight.Value, (int)numericCount.Value + 1);

            //сгенерировать
            generator.Generate(maze, seed,
                (int)numericStairsCount.Value, (int)numericDeleteWalls.Value,
                (int)numericLiftPredel.Value, (int)numericKeyCount.Value,
                checkBoxLayersShuffled.Checked, checkBoxLayers9.Checked,
                checkBoxHolesEnabled.Checked, checkBoxIsLiftInMeat.Checked);
        }

        //отрисовать особняк
        private void DrawMansion(object sender, EventArgs e)
        {
            if (maze != null)
            {
                bool forGame = checkForGame.Checked;

                MazeDrawerConfig config;
                if (forGame) config = new MazeDrawerConfig50();
                else config = new MazeDrawerConfig20();
                config.DrawKeyLayer = checkKeys.Checked;
                config.DrawDecorations = checkDecorations.Checked;

                BitmapMazeDrawer mazeDrawer = new BitmapMazeDrawer(maze, config, forGame);
                pictureBox1.Image = mazeDrawer.Draw(checkLayerStyles.Checked,
                    Utils.GetLayerStyles(maze, (int)numericSeed.Value,
                    checkBoxLayersShuffled.Checked, checkBoxLayers9.Checked));
                pictureBox1.Size = new Size(pictureBox1.Image.Width, pictureBox1.Image.Height);
            }
        }

        private void buttonOpenModel_Click(object sender, EventArgs e)
        {
            if (opendialog.ShowDialog() == DialogResult.OK)
            {
                maze = MultilevelMazeIO.Load(opendialog.FileName);
                DrawMansion(sender, e);
                buttonSaveModel.Enabled = true;
                buttonSaveImage.Enabled = true;
                buttonSaveAtlas.Enabled = true;
            }
        }

        private void buttonSaveModel_Click(object sender, EventArgs e)
        {
            if (savemodel.ShowDialog() == DialogResult.OK)
                MultilevelMazeIO.Save(maze, savemodel.FileName);
        }

        private void buttonSaveImage_Click(object sender, EventArgs e)
        {
            if (saveimage.ShowDialog() == DialogResult.OK)
                pictureBox1.Image.Save(saveimage.FileName);
        }

        private void buttonSaveAtlas_Click(object sender, EventArgs e)
        {
            if (saveimage.ShowDialog() == DialogResult.OK)
            {
                if (maze != null)
                {
                    MazeDrawerConfig config = new MazeDrawerConfig50
                    {
                        DrawKeyLayer = true,
                        DrawDecorations = false
                    };

                    BitmapMazeDrawer mazeDrawer = new BitmapMazeDrawer(maze, config, true);
                    mazeDrawer.DrawAtlas((int)numericAtlasCountX.Value).Save(saveimage.FileName);
                }
            }
        }

        private void checkBoxFixed_CheckedChanged(object sender, EventArgs e)
        {
            numericSeed.Enabled = checkBoxFixed.Checked;
        }

        //вставить параметры режима игры в поля ввода
        private void TemplateIntoInput()
        {
            Template tmpl = templates[currentTemplateIndex];

            //переопределить количество ключей на 16
            if (tmpl.KeyCount == 9 && tmpl.Count > 1)
                tmpl.KeyCount = 16;

            numericWidth.Value = tmpl.Width;
            numericHeight.Value = tmpl.Height;
            numericCount.Value = tmpl.Count;
            numericStairsCount.Value = tmpl.StairsCount;
            numericDeleteWalls.Value = tmpl.DeleteWalls;
            numericLiftPredel.Value = tmpl.LiftPredel;
            numericKeyCount.Value = tmpl.KeyCount;
        }

        private void linkTemplate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            currentTemplateIndex = (int)e.Link.LinkData;
            TemplateIntoInput();
        }

        private void numericCount_ValueChanged(object sender, EventArgs e)
        {
            int value = (int)numericCount.Value;
            numericLiftPredel.Maximum = value + 2;
        }
    }
}
