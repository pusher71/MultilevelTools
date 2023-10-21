using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using MultilevelLibrary;
using MultilevelLibrary.Drawing;
using CamerasChasingData;
using CamerasChasingData.TemplateManagement;
using PrimitiveData3D;

namespace MultilevelViewer
{
    public partial class Form1 : Form
    {
        private GeneratorMain generator; //генератор лабиринтов
        private MultilevelMaze maze; //многоэтажный лабиринт
        private TemplateDiff[] templatesDiff; //список режимов игры
        private int currentTemplateIndex; //номер текущего режима игры
        private int currentDifficulty; //текущая сложность

        public Form1()
        {
            InitializeComponent();
            generator = new GeneratorMain();

            //загрузить режимы игры
            FileTemplateStorage templateStorage = new FileTemplateStorage();
            templatesDiff = templateStorage.LoadTemplates("TemplatesData.txt")
                .Concat(templateStorage.LoadTemplates("TemplatesDataDebug.txt")).ToArray();

            //создать гиперссылки для них
            int currentLocationY = 16;
            for (int i = 0; i < templatesDiff.Length; i++)
            {
                LinkLabel linkTemplate = new LinkLabel
                {
                    Location = new Point(6, currentLocationY),
                    Text = templatesDiff[i].Id,
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
            currentDifficulty = 0; //Easy
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
                (int)numericSafetyCount.Value, (int)numericSavePeriod.Value, (int)numericRadarCount.Value, (int)numericStairsCount.Value, (int)numericDeleteWalls.Value,
                (int)numericLiftPredel.Value, (int)numericKeyCount.Value, (int)numericBottlesCount.Value, (int)numericCamerasCount.Value, checkBoxEnableSafety.Checked,
                checkBoxLayersShuffled.Checked, checkBoxLayers9.Checked, checkBoxHolesEnabled.Checked, checkBoxCamerasEnabled.Checked);

            //проверить его на ряд условий
            if (conditionSaveFloor.Checked && GetItemCountOnFloor(maze, (int)numericSaveFloor.Value - 1, (i) => Utils.IsSaveRoom(i)) == 0)
                throw new ConditionException("Условие conditionSaveFloor не сработало.");
            if (conditionRadarFloor.Checked && GetItemCountOnFloor(maze, (int)numericRadarFloor.Value - 1, (i) => Utils.IsRadarRoom(i)) == 0)
                throw new ConditionException("Условие conditionRadarFloor не сработало.");
        }

        //посчитать элементы на этаже, соответствующие условию
        private int GetItemCountOnFloor(MultilevelMaze m, int floor, Predicate<int> condition)
        {
            int count = 0;
            for (int y = 0; y < m.Height; y++)
                for (int x = 0; x < m.Width; x++)
                    if (condition(m.Map.Get(new Vector3P(x, y, floor) * 2 + 1)))
                        count++;
            return count;
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
                config.DrawSafetyRoomsType = checkRooms.Checked;
                config.DrawKeyLayer = checkKeys.Checked;
                config.DrawDecorations = checkDecorations.Checked;

                BitmapMazeDrawer mazeDrawer = new BitmapMazeDrawer(maze, config, forGame);
                pictureBox1.Image = mazeDrawer.Draw();
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
                        DrawSafetyRoomsType = true,
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
            Template tmpl = templatesDiff[currentTemplateIndex].GetByDifficulty(currentDifficulty);

            numericWidth.Value = tmpl.Width;
            numericHeight.Value = tmpl.Height;
            numericCount.Value = tmpl.Count;
            numericSafetyCount.Value = tmpl.SafetyCount;
            numericSavePeriod.Value = tmpl.SavePeriod;
            numericRadarCount.Value = tmpl.RadarCount;
            numericStairsCount.Value = tmpl.StairsCount;
            numericDeleteWalls.Value = tmpl.DeleteWalls;
            numericLiftPredel.Value = tmpl.LiftPredel;
            numericKeyCount.Value = tmpl.KeyCount;
            numericBottlesCount.Value = tmpl.BottleCount;
            numericCamerasCount.Value = tmpl.CamerasCount;
            checkBoxEnableSafety.Checked = tmpl.EnableSafety;
        }

        private void linkTemplate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            currentTemplateIndex = (int)e.Link.LinkData;
            TemplateIntoInput();
        }

        private void radioEasy_CheckedChanged(object sender, EventArgs e)
        {
            currentDifficulty = 0;
            TemplateIntoInput();
        }

        private void radioNormal_CheckedChanged(object sender, EventArgs e)
        {
            currentDifficulty = 1;
            TemplateIntoInput();
        }

        private void radioHard_CheckedChanged(object sender, EventArgs e)
        {
            currentDifficulty = 2;
            TemplateIntoInput();
        }

        private void numericCount_ValueChanged(object sender, EventArgs e)
        {
            int value = (int)numericCount.Value;
            numericLiftPredel.Maximum = value + 2;
            numericSaveFloor.Maximum = value;
            numericRadarFloor.Maximum = Utils.GetRadarFloorMax(value, numericLiftPredel.Value > 0) + 1;
        }
    }
}
