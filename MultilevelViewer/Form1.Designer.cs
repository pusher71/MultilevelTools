namespace MultilevelViewer
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.numericSeed = new System.Windows.Forms.NumericUpDown();
            this.checkBoxFixed = new System.Windows.Forms.CheckBox();
            this.saveimage = new System.Windows.Forms.SaveFileDialog();
            this.buttonSaveImage = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.numericKeyCount = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numericLiftPredel = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numericCount = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericDeleteWalls = new System.Windows.Forms.NumericUpDown();
            this.numericStairsCount = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericHeight = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericWidth = new System.Windows.Forms.NumericUpDown();
            this.buttonSaveModel = new System.Windows.Forms.Button();
            this.savemodel = new System.Windows.Forms.SaveFileDialog();
            this.buttonNew = new System.Windows.Forms.Button();
            this.checkKeys = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkDecorations = new System.Windows.Forms.CheckBox();
            this.checkForGame = new System.Windows.Forms.CheckBox();
            this.buttonSaveAtlas = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.numericAtlasCountX = new System.Windows.Forms.NumericUpDown();
            this.buttonOpenModel = new System.Windows.Forms.Button();
            this.opendialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxLayers9 = new System.Windows.Forms.CheckBox();
            this.checkBoxHolesEnabled = new System.Windows.Forms.CheckBox();
            this.checkBoxLayersShuffled = new System.Windows.Forms.CheckBox();
            this.checkBoxIsLiftInMeat = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSeed)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericKeyCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLiftPredel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericDeleteWalls)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStairsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericAtlasCountX)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(374, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(130, 205);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // numericSeed
            // 
            this.numericSeed.Enabled = false;
            this.numericSeed.Location = new System.Drawing.Point(230, 97);
            this.numericSeed.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numericSeed.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.numericSeed.Name = "numericSeed";
            this.numericSeed.Size = new System.Drawing.Size(120, 20);
            this.numericSeed.TabIndex = 4;
            this.numericSeed.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // checkBoxFixed
            // 
            this.checkBoxFixed.AutoSize = true;
            this.checkBoxFixed.Location = new System.Drawing.Point(9, 98);
            this.checkBoxFixed.Name = "checkBoxFixed";
            this.checkBoxFixed.Size = new System.Drawing.Size(164, 17);
            this.checkBoxFixed.TabIndex = 6;
            this.checkBoxFixed.Text = "Сид числового генератора:";
            this.checkBoxFixed.UseVisualStyleBackColor = true;
            this.checkBoxFixed.CheckedChanged += new System.EventHandler(this.checkBoxFixed_CheckedChanged);
            // 
            // saveimage
            // 
            this.saveimage.Filter = "PNG images|*.png";
            // 
            // buttonSaveImage
            // 
            this.buttonSaveImage.Enabled = false;
            this.buttonSaveImage.Location = new System.Drawing.Point(12, 488);
            this.buttonSaveImage.Name = "buttonSaveImage";
            this.buttonSaveImage.Size = new System.Drawing.Size(120, 23);
            this.buttonSaveImage.TabIndex = 7;
            this.buttonSaveImage.Text = "Сохранить картинку";
            this.buttonSaveImage.UseVisualStyleBackColor = true;
            this.buttonSaveImage.Click += new System.EventHandler(this.buttonSaveImage_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.numericKeyCount);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.numericLiftPredel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.numericCount);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.checkBoxFixed);
            this.groupBox1.Controls.Add(this.numericDeleteWalls);
            this.groupBox1.Controls.Add(this.numericSeed);
            this.groupBox1.Controls.Add(this.numericStairsCount);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericHeight);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericWidth);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(356, 383);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 203);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(197, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Предпочитаемое количество ключей:";
            // 
            // numericKeyCount
            // 
            this.numericKeyCount.Location = new System.Drawing.Point(300, 201);
            this.numericKeyCount.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numericKeyCount.Name = "numericKeyCount";
            this.numericKeyCount.Size = new System.Drawing.Size(50, 20);
            this.numericKeyCount.TabIndex = 17;
            this.numericKeyCount.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 177);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Высота лифта:";
            // 
            // numericLiftPredel
            // 
            this.numericLiftPredel.Location = new System.Drawing.Point(300, 175);
            this.numericLiftPredel.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numericLiftPredel.Name = "numericLiftPredel";
            this.numericLiftPredel.Size = new System.Drawing.Size(50, 20);
            this.numericLiftPredel.TabIndex = 15;
            this.numericLiftPredel.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Количество этажей:";
            // 
            // numericCount
            // 
            this.numericCount.Location = new System.Drawing.Point(300, 71);
            this.numericCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericCount.Name = "numericCount";
            this.numericCount.Size = new System.Drawing.Size(50, 20);
            this.numericCount.TabIndex = 13;
            this.numericCount.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.numericCount.ValueChanged += new System.EventHandler(this.numericCount_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(203, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Количество удаляемых стен на этаже:";
            // 
            // numericDeleteWalls
            // 
            this.numericDeleteWalls.Location = new System.Drawing.Point(300, 149);
            this.numericDeleteWalls.Maximum = new decimal(new int[] {
            202,
            0,
            0,
            0});
            this.numericDeleteWalls.Name = "numericDeleteWalls";
            this.numericDeleteWalls.Size = new System.Drawing.Size(50, 20);
            this.numericDeleteWalls.TabIndex = 11;
            this.numericDeleteWalls.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // numericStairsCount
            // 
            this.numericStairsCount.Location = new System.Drawing.Point(300, 123);
            this.numericStairsCount.Maximum = new decimal(new int[] {
            112,
            0,
            0,
            0});
            this.numericStairsCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericStairsCount.Name = "numericStairsCount";
            this.numericStairsCount.Size = new System.Drawing.Size(50, 20);
            this.numericStairsCount.TabIndex = 10;
            this.numericStairsCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(256, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Количество лестниц между соседними этажами:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Ширина:";
            // 
            // numericHeight
            // 
            this.numericHeight.Location = new System.Drawing.Point(300, 45);
            this.numericHeight.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericHeight.Name = "numericHeight";
            this.numericHeight.Size = new System.Drawing.Size(50, 20);
            this.numericHeight.TabIndex = 2;
            this.numericHeight.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Длина:";
            // 
            // numericWidth
            // 
            this.numericWidth.Location = new System.Drawing.Point(300, 19);
            this.numericWidth.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericWidth.Name = "numericWidth";
            this.numericWidth.Size = new System.Drawing.Size(50, 20);
            this.numericWidth.TabIndex = 0;
            this.numericWidth.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // buttonSaveModel
            // 
            this.buttonSaveModel.Enabled = false;
            this.buttonSaveModel.Location = new System.Drawing.Point(12, 459);
            this.buttonSaveModel.Name = "buttonSaveModel";
            this.buttonSaveModel.Size = new System.Drawing.Size(120, 23);
            this.buttonSaveModel.TabIndex = 9;
            this.buttonSaveModel.Text = "Сохранить модель";
            this.buttonSaveModel.UseVisualStyleBackColor = true;
            this.buttonSaveModel.Click += new System.EventHandler(this.buttonSaveModel_Click);
            // 
            // savemodel
            // 
            this.savemodel.Filter = "Multilevel maze|*.txt";
            // 
            // buttonNew
            // 
            this.buttonNew.Location = new System.Drawing.Point(12, 401);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(120, 23);
            this.buttonNew.TabIndex = 12;
            this.buttonNew.Text = "Сгенерировать";
            this.buttonNew.UseVisualStyleBackColor = true;
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // checkKeys
            // 
            this.checkKeys.AutoSize = true;
            this.checkKeys.Checked = true;
            this.checkKeys.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkKeys.Location = new System.Drawing.Point(138, 434);
            this.checkKeys.Name = "checkKeys";
            this.checkKeys.Size = new System.Drawing.Size(190, 17);
            this.checkKeys.TabIndex = 13;
            this.checkKeys.Text = "Показывать ключевые объекты";
            this.checkKeys.UseVisualStyleBackColor = true;
            this.checkKeys.CheckedChanged += new System.EventHandler(this.DrawMansion);
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(12, 517);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(174, 253);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Режимы игры";
            // 
            // checkDecorations
            // 
            this.checkDecorations.AutoSize = true;
            this.checkDecorations.Location = new System.Drawing.Point(138, 463);
            this.checkDecorations.Name = "checkDecorations";
            this.checkDecorations.Size = new System.Drawing.Size(147, 17);
            this.checkDecorations.TabIndex = 17;
            this.checkDecorations.Text = "Показывать украшения";
            this.checkDecorations.UseVisualStyleBackColor = true;
            this.checkDecorations.CheckedChanged += new System.EventHandler(this.DrawMansion);
            // 
            // checkForGame
            // 
            this.checkForGame.AutoSize = true;
            this.checkForGame.Location = new System.Drawing.Point(138, 492);
            this.checkForGame.Name = "checkForGame";
            this.checkForGame.Size = new System.Drawing.Size(130, 17);
            this.checkForGame.TabIndex = 19;
            this.checkForGame.Text = "Отрисовка для игры";
            this.checkForGame.UseVisualStyleBackColor = true;
            this.checkForGame.CheckedChanged += new System.EventHandler(this.DrawMansion);
            // 
            // buttonSaveAtlas
            // 
            this.buttonSaveAtlas.Enabled = false;
            this.buttonSaveAtlas.Location = new System.Drawing.Point(50, 45);
            this.buttonSaveAtlas.Name = "buttonSaveAtlas";
            this.buttonSaveAtlas.Size = new System.Drawing.Size(120, 23);
            this.buttonSaveAtlas.TabIndex = 20;
            this.buttonSaveAtlas.Text = "Сохранить атлас";
            this.buttonSaveAtlas.UseVisualStyleBackColor = true;
            this.buttonSaveAtlas.Click += new System.EventHandler(this.buttonSaveAtlas_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonSaveAtlas);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.numericAtlasCountX);
            this.groupBox4.Location = new System.Drawing.Point(192, 619);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(176, 74);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Атлас";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 21);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(95, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Ширина таблицы:";
            // 
            // numericAtlasCountX
            // 
            this.numericAtlasCountX.Location = new System.Drawing.Point(120, 19);
            this.numericAtlasCountX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericAtlasCountX.Name = "numericAtlasCountX";
            this.numericAtlasCountX.Size = new System.Drawing.Size(50, 20);
            this.numericAtlasCountX.TabIndex = 0;
            this.numericAtlasCountX.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // buttonOpenModel
            // 
            this.buttonOpenModel.Location = new System.Drawing.Point(12, 430);
            this.buttonOpenModel.Name = "buttonOpenModel";
            this.buttonOpenModel.Size = new System.Drawing.Size(120, 23);
            this.buttonOpenModel.TabIndex = 20;
            this.buttonOpenModel.Text = "Открыть модель";
            this.buttonOpenModel.UseVisualStyleBackColor = true;
            this.buttonOpenModel.Click += new System.EventHandler(this.buttonOpenModel_Click);
            // 
            // opendialog
            // 
            this.opendialog.Filter = "Multilevel maze|*.txt";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBoxIsLiftInMeat);
            this.groupBox5.Controls.Add(this.checkBoxLayers9);
            this.groupBox5.Controls.Add(this.checkBoxHolesEnabled);
            this.groupBox5.Controls.Add(this.checkBoxLayersShuffled);
            this.groupBox5.Location = new System.Drawing.Point(192, 517);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(176, 96);
            this.groupBox5.TabIndex = 21;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Experimental";
            // 
            // checkBoxLayers9
            // 
            this.checkBoxLayers9.AutoSize = true;
            this.checkBoxLayers9.Location = new System.Drawing.Point(132, 19);
            this.checkBoxLayers9.Name = "checkBoxLayers9";
            this.checkBoxLayers9.Size = new System.Drawing.Size(32, 17);
            this.checkBoxLayers9.TabIndex = 3;
            this.checkBoxLayers9.Text = "9";
            this.checkBoxLayers9.UseVisualStyleBackColor = true;
            // 
            // checkBoxHolesEnabled
            // 
            this.checkBoxHolesEnabled.AutoSize = true;
            this.checkBoxHolesEnabled.Location = new System.Drawing.Point(6, 42);
            this.checkBoxHolesEnabled.Name = "checkBoxHolesEnabled";
            this.checkBoxHolesEnabled.Size = new System.Drawing.Size(96, 17);
            this.checkBoxHolesEnabled.TabIndex = 1;
            this.checkBoxHolesEnabled.Text = "Дырки в полу";
            this.checkBoxHolesEnabled.UseVisualStyleBackColor = true;
            // 
            // checkBoxLayersShuffled
            // 
            this.checkBoxLayersShuffled.AutoSize = true;
            this.checkBoxLayersShuffled.Location = new System.Drawing.Point(6, 19);
            this.checkBoxLayersShuffled.Name = "checkBoxLayersShuffled";
            this.checkBoxLayersShuffled.Size = new System.Drawing.Size(120, 17);
            this.checkBoxLayersShuffled.TabIndex = 0;
            this.checkBoxLayersShuffled.Text = "Слои перемешаны";
            this.checkBoxLayersShuffled.UseVisualStyleBackColor = true;
            // 
            // checkBoxIsLiftInMeat
            // 
            this.checkBoxIsLiftInMeat.AutoSize = true;
            this.checkBoxIsLiftInMeat.Location = new System.Drawing.Point(6, 65);
            this.checkBoxIsLiftInMeat.Name = "checkBoxIsLiftInMeat";
            this.checkBoxIsLiftInMeat.Size = new System.Drawing.Size(91, 17);
            this.checkBoxIsLiftInMeat.TabIndex = 4;
            this.checkBoxIsLiftInMeat.Text = "Лифт в мясе";
            this.checkBoxIsLiftInMeat.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 782);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.buttonOpenModel);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.checkForGame);
            this.Controls.Add(this.checkDecorations);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkKeys);
            this.Controls.Add(this.buttonNew);
            this.Controls.Add(this.buttonSaveModel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonSaveImage);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Генератор помещений";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSeed)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericKeyCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLiftPredel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericDeleteWalls)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStairsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericAtlasCountX)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.NumericUpDown numericSeed;
        private System.Windows.Forms.CheckBox checkBoxFixed;
        private System.Windows.Forms.SaveFileDialog saveimage;
        private System.Windows.Forms.Button buttonSaveImage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericStairsCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericWidth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericDeleteWalls;
        private System.Windows.Forms.Button buttonSaveModel;
        private System.Windows.Forms.SaveFileDialog savemodel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericCount;
        private System.Windows.Forms.NumericUpDown numericLiftPredel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonNew;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericKeyCount;
        private System.Windows.Forms.CheckBox checkKeys;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkDecorations;
        private System.Windows.Forms.CheckBox checkForGame;
        private System.Windows.Forms.Button buttonSaveAtlas;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numericAtlasCountX;
        private System.Windows.Forms.Button buttonOpenModel;
        private System.Windows.Forms.OpenFileDialog opendialog;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox checkBoxHolesEnabled;
        private System.Windows.Forms.CheckBox checkBoxLayersShuffled;
        private System.Windows.Forms.CheckBox checkBoxLayers9;
        private System.Windows.Forms.CheckBox checkBoxIsLiftInMeat;
    }
}

