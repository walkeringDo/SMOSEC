using System;
using System.Data;
using Smobiler.Core.Controls;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmConQuant : Smobiler.Core.Controls.MobileForm
    {
        #region ����
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������
        public string CID;
        #endregion
        /// <summary>
        /// ������˻��˼�����رմ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConQuant_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
                Close();
        }

        /// <summary>
        /// �����ʼ��ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConQuant_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dataTable = _autofacConfig.ConsumablesService.GetQuants("", CID);
                if (dataTable.Rows.Count > 0)
                {
                    GridView1.DataSource = dataTable;
                    GridView1.DataBind();
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
    }
}