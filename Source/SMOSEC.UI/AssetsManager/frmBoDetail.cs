using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SMOSEC.DTOs.OutputDTO;
using Smobiler.Core;
using Smobiler.Core.Controls;
using ListView = Smobiler.Core.Controls.ListView;

namespace SMOSEC.UI.AssetsManager
{
    partial class frmBoDetail : Smobiler.Core.Controls.MobileForm
    {
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������

        public string BoId;  //���õ����
        /// <summary>
        /// �����˼�����رմ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmBODetail_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
                Close();
        }

        /// <summary>
        /// �����ʼ��ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmBODetail_Load(object sender, EventArgs e)
        {
            Bind();
        }

        /// <summary>
        /// ������
        /// </summary>
        public void Bind()
        {
            try
            {
                AssBorrowOrderOutputDto assBorrowOrderOutput = _autofacConfig.AssetsService.GetBobyId(BoId);
                if (assBorrowOrderOutput != null)
                {
                    txtBOMan.Text = assBorrowOrderOutput.Borrower;
                    txtHMan.Text = assBorrowOrderOutput.Brhandleman;
                    txtLocation.Text = assBorrowOrderOutput.Locationid;
                    txtNote.Text = assBorrowOrderOutput.Note;
                    DPickerCO.Value = assBorrowOrderOutput.Borrowdate;
                    DPickerRs.Value = assBorrowOrderOutput.Eptreturndate;
                }
                DataTable rowsTable = _autofacConfig.AssetsService.GetRowsByBoid(BoId);
                if (rowsTable != null)
                {
                    ListAss.DataSource = rowsTable;
                    ListAss.DataBind();
                }
                
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
    }
}