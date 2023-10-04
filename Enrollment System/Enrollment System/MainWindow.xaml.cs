using CsvHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace Enrollment_System
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        List<TeachersName> teachers = new List<TeachersName>();
        List<Teacher> courses = new List<Teacher>();
        List<Student> students = new List<Student>();
        List<Choose> chooses = new List<Choose>();
        Teacher selected_Course;
        Student selected_Student;
        Choose selected_Record;
        string NumCredits = string.Empty;//篩選學分字串
        string oCompulsory = string.Empty;//篩選必選修字串
        public MainWindow()
        {
            InitializeComponent();
        }
        List<TC> CsvDataReader<TC>(string path)
        {
            CsvReader csvr = new CsvReader(new StreamReader(path,Encoding.Default), CultureInfo.InvariantCulture);
            csvr.Configuration.HeaderValidated = null;
            csvr.Configuration.MissingFieldFound = null;
            return csvr.GetRecords<TC>().ToList();
        }
        private void windows_Loaded(object sender, RoutedEventArgs e)
        {
            courses = CsvDataReader<Teacher>("./course.csv"); //老師收尋資料路徑，創建teacher list
            students = CsvDataReader<Student>("./2B.csv"); //學生收尋資料路徑，創建student list
            foreach (Teacher tc in courses)
            {
                var q = teachers.IndexOf(teachers.Where(X => X.TeacherName ==tc.TeacherName).FirstOrDefault()); //取得root點
                if (q > -1)
                {
                    teachers[q].Course.Add(tc); //將除了老師姓名放入course變數裡
                }
                else
                {
                    teachers.Add(new TeachersName(tc.TeacherName, tc)); //建立treelist 的root點
                }
            }
            Teachertreeview.ItemsSource = teachers; //將資料載入listview
            Teacherlistview.ItemsSource = courses; //將資料載入treeview
            comboBox.ItemsSource = students; //將資料載入combobox
            CollectionView view = (CollectionView)
           CollectionViewSource.GetDefaultView
           (Teacherlistview.ItemsSource);
            view.Filter = UserFilter;
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            selected_Student = comboBox.SelectedItem as Student; //將combobox所選的學生寫入變數裡
        if (selected_Student is null || selected_Course is null) //判斷是否有未選擇課程或學生
        {
                MessageBox.Show("未選擇課程或學生", "錯誤");
                goto end;
            }
            if (Teachertreeview.SelectedItem is Teacher)//判斷是在treeview 或是在lsitview
            {
                selected_Course = Teachertreeview.SelectedItem as Teacher;
            }
            else
            {
                selected_Course = Teacherlistview.SelectedItem as Teacher;
            }
            for (int i = 0; i < chooses.Count; i++) //判斷是否重複選課
            {
                if (selected_Student.StudentName == chooses[i].Name && selected_Course.CourseTitle == chooses[i].SubjectName)
                {
                    MessageBox.Show("重複選取", "錯誤");
                    goto end;
                }
            }
            chooses.Add(new Choose()
            {
                Name = selected_Student.StudentName,//學生
                StudentID = selected_Student.StudentNum,//學號
                Instructor = selected_Course.TeacherName,//任課老師
                SubjectName = selected_Course.CourseTitle,//課程名稱
                Compulsory = selected_Course.Compulsory,//必選修
                Credits = selected_Course.Credits,//學分
                OpeningClass = selected_Course.Openingclass//開課班級
            }
            );

            listRegistration.ItemsSource = chooses;
            listRegistration.Items.Refresh();//重新載入資料到listview
        end:
            StatusLable.Content = ($"請重新選擇課程或學生");
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e) //刪除加退選
        {
            if (listRegistration.SelectedValue == null)
            {
                MessageBox.Show("未選擇退選課程", "錯誤");
            }
            else
            {
                int gg = listRegistration.SelectedIndex;//讀取選擇的位置
                chooses.RemoveAt(gg);//將chooses[gg]移除
                listRegistration.Items.Refresh();//重新載入選課清單
            }
 
        }
        private void save_Click(object sender, RoutedEventArgs e)
        {
            int totalCredits = 0, totalclass = 0;
            if (chooses.Count == 0)
            {
                MessageBox.Show("未加選課");
                StatusLable.Content = ($"未加選課");
    }
            else
    {
        var path = string.Empty;//用來儲存路徑
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "csv files (*.csv)|*.csv",
            FileName = chooses[0].StudentID + chooses[0].Name + "選課清單",//預設名稱為第一列學號+學生姓名
            DefaultExt = "csv",
            AddExtension = true
        };
        saveFileDialog.ShowDialog();
        path = saveFileDialog.FileName;
        FileStream fs = new FileStream(path,
       System.IO.FileMode.Create, System.IO.FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs,
       System.Text.Encoding.UTF8);
        string data = string.Empty;
        for (int i = 0; i < chooses.Count; i++)
        {
            totalCredits += Int32.Parse(chooses[i].Credits);
            totalclass = i;
        }
        //加入第一列
        data = "\"" + "學號" + "\"" + "," + "\"" + "學生姓名" + "\"" +
       "," + "\"" + "選課總科樹" + "\"" + "," + "\"" + "學分數" +
       "\"";
        sw.WriteLine(data);//寫入date資料中
        data = "\"" + chooses[0].Name + "\"" + "," + "\"" + chooses
       [0].StudentID + "\"" + "," + "\"" + totalCredits + "\"" +
       "," + "\"" + totalclass + "\"";
        sw.WriteLine(data);
        data = "\"" + "學號" + "\"" + "," + "\"" + "學生姓名" + "\"" +
       "," + "\"" + "授課老師" + "\"" + "," + "\"" + "科目名稱" +
       "\"" + "," + "\"" + "必選修" + "\"" + "," + "\"" + "學分數"
       + "\"" + "," + "\"" + "開課班級" + "\"";
        sw.WriteLine(data);
        for (int i = 0; i < chooses.Count; i++)//一次加入一列
        {
            data = string.Empty;//以逗號跟冒號隔開(例"Name","StudentID"data = "\"" + chooses[i].Name + "\"" + "," + "\"" +chooses[i].StudentID + "\"" + "," + "\"" + chooses[i].Instructor + "\"" + "," + "\"" + chooses[i].SubjectName+ "\"" + "," + "\"" + chooses[i].Compulsory + "\"" + "," +"\"" + chooses[i].Credits + "\"" + "," + "\"" + chooses[i].OpeningClass + "\"";sw.WriteLine(data);
        }
        sw.Close();
        fs.Close();
        }
    }
    private void TeacherTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) //status顯示設定 教師treeview
        {
            if (Teachertreeview.SelectedItem is Teacher)
            {
                selected_Course = Teachertreeview.SelectedItem as Teacher; //轉型
                StatusLable.Content = $"任課教師:{selected_Course.TeacherName.ToString()} ||{selected_Course.ToString()}";//在staus顯示目前選的課程
            }
    }
private void TeachersListViewSelectionChanged(object sender,
SelectionChangedEventArgs e) //status顯示設定 教師listview
{
    if (Teacherlistview.SelectedItem is Teacher)
    {
                selected_Course = Teacherlistview.SelectedItem as Teacher; //轉型
                StatusLable.Content = $"任課教師:{ selected_Course.TeacherName.ToString()} ||{ selected_Course.ToString()}";//在staus顯示目前選的課程
    }
}
private void listRegistrationSelectionChanged(object sender,
SelectionChangedEventArgs e) //status顯示設定 選課listview
{
    if (listRegistration.SelectedItem is Choose)
    {
        selected_Record = listRegistration.SelectedItem as Choose;
        StatusLable.Content = $"{selected_Record.ToString()}"; //在staus顯示目前選的課程
    }
}
//===================================================================================================================================
// 篩選
//===================================================================================================================================
private bool UserFilter(object item)//只有傳回true的課程會被顯示
        {
            if (string.IsNullOrEmpty(NumCredits) && string.IsNullOrEmpty(oCompulsory))//檢查學分數和必選修是否為空
                return true;
            else if (string.IsNullOrEmpty(oCompulsory))//檢查是否只有必選修為空
                return ((item as Teacher).Credits == NumCredits);//只要課程學分跟cBoxCredits一樣即傳回true
            else if (string.IsNullOrEmpty(NumCredits))//檢查是否只有
                return ((item as Teacher).Compulsory == NumCredits);//只要課程必選修跟cBoxCompulsory一樣即傳回true
            else
                return ((item as Teacher).Credits == NumCredits && (item as Teacher).Compulsory == oCompulsory);//課程跟必選修都要一樣才傳回true
        }
        private void Elective_Checked(object sender, RoutedEventArgs e)
        {
 if (courses.Count > 0)
 {
 if (Elective.IsChecked == true)
 {
 Departmentelective.IsChecked = false; //將其餘關閉只顯示選修
 Compulsory.IsChecked = false;
 oCompulsory = "選修";
 CollectionViewSource.GetDefaultView
(Teacherlistview.ItemsSource).Refresh(); //設定要篩選的文字或數字，再重新載入listview
 }
 else if (Elective.IsChecked == false)
 {
 oCompulsory = string.Empty;
 CollectionViewSource.GetDefaultView
(Teacherlistview.ItemsSource).Refresh();//設定要篩選的文字或數字，再重新載入listview
 }
 }
 else
 MessageBox.Show("未加入課程");
 }
        private void Compulsory_Checked(object sender, RoutedEventArgs e)
        {
            if (courses.Count > 0)
 {
 if (Compulsory.IsChecked == true)
 {
 Elective.IsChecked = false; //將其餘關閉只顯示必修
 Departmentelective.IsChecked = false;
 oCompulsory = "必修";
 CollectionViewSource.GetDefaultView
(Teacherlistview.ItemsSource).Refresh();//設定要篩選的文字或數字，再重新載入listview
 }
 else if (Compulsory.IsChecked == false)
 {
 oCompulsory = string.Empty;
 CollectionViewSource.GetDefaultView
 (Teacherlistview.ItemsSource).Refresh();//設定要篩選的文字或數字，再重新載入listview
 }
 }
 else
 MessageBox.Show("未加入課程");
 }
 private void Departmentelective_Checked(object sender, RoutedEventArgs
e)
 {
 if (courses.Count > 0)
 {
 if (Departmentelective.IsChecked == true)
 {
 Elective.IsChecked = false; //將其餘關閉只顯示系定選修
 Compulsory.IsChecked = false;
 oCompulsory = "系定選修";
 CollectionViewSource.GetDefaultView
(Teacherlistview.ItemsSource).Refresh();//設定要篩選的文字或數字，再重新載入listview
 }
 else if (Departmentelective.IsChecked == false)
 {
 oCompulsory = string.Empty;
 CollectionViewSource.GetDefaultView
(Teacherlistview.ItemsSource).Refresh();//設定要篩選的文字或數字，再重新載入listview
 }
 }
 else
 MessageBox.Show("未加入課程");
 }
 private void threeCredits_Checked(object sender, RoutedEventArgs e)
 {
 if (courses.Count > 0)
 {
 if (threeCredits.IsChecked == true)
 {
 twoCredits.IsChecked = false; //將其餘關閉只顯示3學分的課程
 oneCredits.IsChecked = false;
 NumCredits = "3";
 CollectionViewSource.GetDefaultView
(Teacherlistview.ItemsSource).Refresh();//設定要篩選的文字或數字，再重新載入listview
 }
 else if (threeCredits.IsChecked == false)
 {
 NumCredits = string.Empty;
 CollectionViewSource.GetDefaultView
(Teacherlistview.ItemsSource).Refresh();//設定要篩選的文字或數字，再重新載入listview
 }
 }
 else
 MessageBox.Show("未加入課程");
 }
 private void twoCredits_Checked(object sender, RoutedEventArgs e)
 {
 if (courses.Count > 0)
 {
 if (twoCredits.IsChecked == true)
 {
 oneCredits.IsChecked = false; //將其餘關閉只顯示2學分的課程
 threeCredits.IsChecked = false;
 NumCredits = "2";
 CollectionViewSource.GetDefaultView
(Teacherlistview.ItemsSource).Refresh();//設定要篩選的文字或數字，再重新載入listview
 }
 else if (twoCredits.IsChecked == false)
 {
 NumCredits = string.Empty;
 CollectionViewSource.GetDefaultView
(Teacherlistview.ItemsSource).Refresh();//設定要篩選的文字或數字，再重新載入listview
 }
 }
 else
 MessageBox.Show("未加入課程");
 }
 private void oneCredits_Checked(object sender, RoutedEventArgs e)
 {
 if (courses.Count > 0)
 {
 if (oneCredits.IsChecked == true)
 {
 twoCredits.IsChecked = false; //將其餘關閉只顯示1學分的課程
 threeCredits.IsChecked = false;
 NumCredits = "1";
 CollectionViewSource.GetDefaultView
(Teacherlistview.ItemsSource).Refresh();//設定要篩選的文字或數字，再重新載入listview
 }
 else if (oneCredits.IsChecked == false)
 {
 NumCredits = string.Empty;
 CollectionViewSource.GetDefaultView
(Teacherlistview.ItemsSource).Refresh();//設定要篩選的文字或數字，再重新載入listview
}
 }
 else
 MessageBox.Show("未加入課程");
 }
 }
 }
