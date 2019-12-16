using System;
using System.Data;
using SMOSEC.DTOs.OutputDTO;
using Smobiler.Core.Controls;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmOutOrderDetail : Smobiler.Core.Controls.MobileForm
    {
        #region ����
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������      
        public string OOId;
        

        #endregion

        /// <summary>
        /// ������ʱ���رյ�ǰ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmOutOrderDetail_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
                Close();
        }

        /// <summary>
        /// ��ʼ������ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmOutOrderDetail_Load(object sender, EventArgs e)
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
                OutboundOrderOutputDto outboundOrder = _autofacConfig.ConsumablesService.GetOutboundOrderById(OOId);
                if (outboundOrder != null)
                {
                    txtType.Text = outboundOrder.TYPENAME;
                    txtHMan.Text = outboundOrder.HANDLEMANNAME;
                    txtLocation.Text = outboundOrder.LOCATIONNAME;
                    txtNote.Text = outboundOrder.NOTE;
                    DPickerCO.Value = outboundOrder.BUSINESSDATE;
                    txtType.Text = outboundOrder.TYPE == 1 ? "�˻�" : "����";
                }

                DataTable rowsTable = _autofacConfig.ConsumablesService.GetOORowListByOOId(OOId);
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