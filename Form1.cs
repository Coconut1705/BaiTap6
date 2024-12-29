using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using BaiTap6.Models;


namespace BaiTap6
{
    public partial class frmStudent : Form
    {
        public frmStudent()
        {
            InitializeComponent();
        }

        private void txtStudentID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtFullName_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtAverageScore_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                StudentContextDB context = new StudentContextDB();
                List<Student> studentList = context.Students.ToList();
                if (studentList.Any(s => s.StudentID == txtStudentID.Text))
                {
                    MessageBox.Show("Mã SV đã tồn tại. Vui lòng nhập một mã khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var newStudent = new Student
                {
                    StudentID = txtStudentID.Text,
                    FullName = txtFullName.Text,
                    AverageScore = double.Parse(txtAverageScore.Text),
                    FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString())
                };

                // Add the new student to the list
                context.Students.Add(newStudent);
                context.SaveChanges();

                // Reload the data
                BindGrid(context.Students.ToList());
                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                StudentContextDB context = new StudentContextDB();
                List<Student> students = context.Students.ToList();

                var student = students.FirstOrDefault(s => s.StudentID == txtStudentID.Text);
                if (student != null)
                {
                    if (students.Any(s => s.StudentID == txtStudentID.Text && s.StudentID != student.StudentID))
                    {
                        MessageBox.Show("Mã số đã tồn tại. Vui lòng nhập một mã khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    student.FullName = txtFullName.Text;
                    student.AverageScore = double.Parse(txtAverageScore.Text);
                    student.FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString());

                    // Save changes to the database
                    context.SaveChanges();

                    // Reload the data
                    BindGrid(context.Students.ToList());
                    MessageBox.Show("Chỉnh sửa thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sinh viên không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                StudentContextDB context = new StudentContextDB();
                List<Student> studentList = context.Students.ToList();

                // Find the student by Student ID
                var student = studentList.FirstOrDefault(s => s.StudentID == txtStudentID.Text);
                if (student != null)
                {
                    // Remove the student from the list
                    context.Students.Remove(student);
                    context.SaveChanges();

                    BindGrid(context.Students.ToList());
                    MessageBox.Show("Sinh viên đã được xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sinh viên không tìm thấy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Kiểm tra nếu người dùng click vào một dòng hợp lệ
                if (e.RowIndex >= 0)
                {
                    // Lấy dữ liệu từ dòng được chọn
                    DataGridViewRow row = dgvStudent.Rows[e.RowIndex];

                    // Điền thông tin vào các trường nhập liệu
                    txtStudentID.Text = row.Cells[0].Value.ToString();  // Mã sinh viên
                    txtFullName.Text = row.Cells[1].Value.ToString();   // Tên sinh viên
                    txtAverageScore.Text = row.Cells[3].Value.ToString(); // Điểm trung bình

                    // Điền thông tin vào ComboBox (chọn khoa tương ứng)
                    string facultyName = row.Cells[2].Value.ToString(); // Tên khoa từ DataGridView
                    cmbFaculty.SelectedIndex = cmbFaculty.Items.Cast<Faculty>().ToList().FindIndex(f => f.FacultyName == facultyName); // Chọn khoa theo tên
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận trước khi thoát
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Nếu người dùng chọn "Yes", đóng form
                this.Close();
            }
            // Nếu người dùng chọn "No", không làm gì, form sẽ vẫn mở
        }

        private void frmStudent_Load(object sender, EventArgs e)
        {
           
                try
                {
                    StudentContextDB context = new StudentContextDB();
                    List<Faculty> listFalcultys = context.Faculties.ToList(); //lấy các khoa
                    List<Student> listStudent = context.Students.ToList(); //lấy sinh viên
                    FillFalcultyCombobox(listFalcultys);
                    BindGrid(listStudent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        
        private void FillFalcultyCombobox(List<Faculty> listFalcultys)
        {
            this.cmbFaculty.DataSource = listFalcultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore;
            }
        }
    }
}




