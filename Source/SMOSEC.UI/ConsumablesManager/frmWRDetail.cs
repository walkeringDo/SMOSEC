using System;
using System.Data;
using Smobiler.Core.Controls;
using SMOSEC.DTOs.OutputDTO;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmWRDetail : Smobiler.Core.Controls.MobileForm
    {
        #region ����
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������
        public string WRID; //��ⵥ���
        

        #endregion

        /// <summary>
        /// �����˼����رյ�ǰ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmWRDetail_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
                Close();
        }

        /// <summary>
        /// �����ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmWRDetail_Load(object sender, EventArgs e)
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
                WarehouseReceiptOutputDto warehouseReceipt = _autofacConfig.ConsumablesService.GetWarehouseReceiptById(WRID);
                if (warehouseReceipt != null)
                {
                    txtVendor.Text = warehouseReceipt.VENDOR;
                    txtHMan.Text = warehouseReceipt.HANDLEMANNAME;
                    txtLocatin.Text = warehouseReceipt.LOCATIONNAME;
                    txtNote.Text = warehouseReceipt.NOTE;
                    DPickerCO.Value = warehouseReceipt.BUSINESSDATE;
                }

                DataTable rowsTable = _autofacConfig.ConsumablesService.GetWRRowListByWRId(WRID);
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