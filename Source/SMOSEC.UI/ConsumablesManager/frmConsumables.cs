using System;
using System.Data;
using Smobiler.Core.Controls;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmConsumables : Smobiler.Core.Controls.MobileForm
    {
        #region ����
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������
        #endregion

        /// <summary>
        /// ��ת����ӺĲ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Press(object sender, EventArgs e)
        {
            try
            {
                frmConsumablesCreate consumablesCreate=new frmConsumablesCreate();
                Show(consumablesCreate, (MobileForm sender1, object args) =>
                {
                    if (consumablesCreate.ShowResult == ShowResult.Yes)
                    {
                        Bind();
                    }
                });
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// �����ˣ��رտͻ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConsumables_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
                Client.Exit();
        }

        /// <summary>
        /// �����ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConsumables_Load(object sender, EventArgs e)
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
                if (Client.Session["Role"].ToString() == "SMOSECUser")
                {
                    panel1.Visible = false;
                }

                DataTable conTable = _autofacConfig.ConsumablesService.GetConList();
                ListViewCon.Rows.Clear();
                if (conTable.Rows.Count > 0)
                {
                    ListViewCon.DataSource = conTable;
                    ListViewCon.DataBind();
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
    }
}