using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smobiler.Core;
using Smobiler.Core.Controls;
using System.Data;
using System.Drawing;
using SMOSEC.UI.Layout;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmConInventory : Smobiler.Core.Controls.MobileForm
    {
        #region  �������
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������
        #endregion
        /// <summary>
        /// ҳ���ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConInventory_Load(object sender, EventArgs e)
        {
            Bind();
        }
        /// <summary>
        /// ���ݰ�
        /// </summary>
        public void Bind()
        {
            try
            {
                string LocationId = "";
                string UserId = Session["UserID"].ToString();
                if (Client.Session["Role"].ToString() == "SMOSECAdmin")
                {
                    var user = _autofacConfig.coreUserService.GetUserByID(UserId);
                    LocationId = user.USER_LOCATIONID;
                }

                DataTable assInventoryList = _autofacConfig.ConInventoryService.GetConInventoryList(Client.Session["Role"].ToString() == "SMOSECUser" ? Client.Session["UserID"].ToString() : "", LocationId);
                listView.Rows.Clear();
                if (assInventoryList.Rows.Count > 0)
                {
                    listView.DataSource = assInventoryList;
                    listView.DataBind();
                }
                foreach (var row in listView.Rows)
                {
                    frmConInventoryLayout layout = (frmConInventoryLayout)row.Control;
                    switch (layout.label1.Text)
                    {
                        case "�̵����":
                            layout.label1.ForeColor = Color.FromArgb(43, 125, 43);
                            break;
                        case "�̵���":
                            layout.label1.ForeColor = Color.FromArgb(43, 140, 255);
                            layout.btnStart.Text = "�����̵�";
                            break;
                        case "�̵�δ��ʼ":
                            layout.label1.ForeColor = Color.FromArgb(211, 215, 217);
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }

        }
        /// <summary>
        /// �ֻ��Դ����ؼ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConInventory_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
                Client.Exit();
        }
        /// <summary>
        /// �Ĳ��̵㵥����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Press(object sender, EventArgs e)
        {
            try
            {
                frmConInventoryCreate conInventoryCreate = new frmConInventoryCreate();
                Show(conInventoryCreate, (MobileForm sender1, object args) =>
                {
                    if (conInventoryCreate.ShowResult == ShowResult.Yes)
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
    }
}