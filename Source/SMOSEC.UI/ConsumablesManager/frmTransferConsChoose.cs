using System;
using Smobiler.Core.Controls;
using SMOSEC.DTOs.InputDTO;
using System.Data;
using SMOSEC.UI.Layout;
using System.Collections.Generic;
using SMOSEC.Domain.Entity;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmTransferConsChoose : Smobiler.Core.Controls.MobileForm
    {
        #region "definition"
        AutofacConfig autofacConfig = new AutofacConfig();     //����������
        public List<ConsumablesOrderRow> RowData = new List<ConsumablesOrderRow>();    //δ����SN����
        public String LocInID;           //����������
        #endregion
        /// <summary>
        /// �������ƽ��в�ѯ�ʲ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plSearch_Press(object sender, EventArgs e)
        {
            Bind(txtFactor.Text);
        }
        /// <summary>
        /// ȫѡ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Checkall_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if(tpvAssets.PageIndex==0)     //δ����SN�ʲ�
                //{
                foreach (ListViewRow Row in ListAssets.Rows)
                {
                    frmAssetsLayout Layout = Row.Control as frmAssetsLayout;
                    Layout.setCheck(Checkall.Checked);
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ����ȫѡ״̬
        /// </summary>
        public void upCheckState()
        {
            Int32 selectQty = 0;        //��ǰѡ��������
                                        //if (tpvAssets.PageIndex == 0)
                                        //{
            foreach (ListViewRow Row in ListAssets.Rows)
            {
                frmAssetsLayout Layout = Row.Control as frmAssetsLayout;
                selectQty += Layout.checkNum();
            }
            if (selectQty == ListAssets.Rows.Count)
                Checkall.Checked = true;          //ѡ����������ʱ
            else
                Checkall.Checked = false;        //û��ѡ����������
        }
        /// <summary>
        /// ѡ���ʲ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Press(object sender, EventArgs e)
        {
            try
            {
                if (RowData.Count > 0) RowData.Clear();
                foreach (ListViewRow Row in ListAssets.Rows)
                {
                    frmAssetsLayout Layout = Row.Control as frmAssetsLayout;
                    if (Layout.getData() != null)
                    {
                        RowData.Add(Layout.getData());     //���δ����SN�ʲ���Ϣ
                    }
                }
                ShowResult = ShowResult.Yes;
                Form.Close();       //�رյ�ǰҳ��
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// �л���ʾListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tpvAssets_PageIndexChanged(object sender, EventArgs e)
        {
            upCheckState();      //����ȫѡ��״̬
        }
        /// <summary>
        /// ҳ���ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAssetsChoose_Load(object sender, EventArgs e)
        {
            Bind(null);
        }
        /// <summary>
        /// ���ݰ�
        /// </summary>
        /// <param name="Name"></param>
        public void Bind(String Name)
        {
            try
            {
                DataTable tableAssets = new DataTable();       //δ����SN���ʲ��б�
                tableAssets.Columns.Add("CHECK");              //�ʲ����
                tableAssets.Columns.Add("CID");                //�Ĳı��
                tableAssets.Columns.Add("NAME");               //�ʲ����� 
                tableAssets.Columns.Add("LOCATIONID");         //������
                tableAssets.Columns.Add("LOCATIONNAME");       //��������
                tableAssets.Columns.Add("IMAGE");              //ͼƬ���
                tableAssets.Columns.Add("QUANTITY");          //��������
                tableAssets.Columns.Add("SELECTQTY");      //ѡ������

                List<ConQuant> listAss = new List<ConQuant>();
                if (String.IsNullOrEmpty(Name))     //��ѯ���кĲ�
                    listAss = autofacConfig.orderCommonService.GetUnUseCon(LocInID,null);
                else
                {
                    Consumables consumables = autofacConfig.orderCommonService.GetConsByName(Name);
                    listAss = autofacConfig.orderCommonService.GetUnUseCon(LocInID,consumables.CID);
                }
                foreach (ConQuant Row in listAss)   
                {
                    Consumables cons = autofacConfig.orderCommonService.GetConsByID(Row.CID);
                    AssLocation location = autofacConfig.assLocationService.GetByID(Row.LOCATIONID);
                    if (RowData.Count > 0)
                    {
                        Boolean isAdd = false;
                        foreach (ConsumablesOrderRow HaveRow in RowData)
                        {
                            if (HaveRow.CID == Row.CID && HaveRow.LOCATIONID == Row.LOCATIONID)
                            {
                                tableAssets.Rows.Add(true, Row.CID, cons.NAME,Row.LOCATIONID, location.NAME, cons.IMAGE,Row.QUANTITY,HaveRow.QTY);
                                isAdd = true;
                                break;
                            }
                        }
                        if (isAdd == false)
                            tableAssets.Rows.Add(false, Row.CID, cons.NAME, Row.LOCATIONID, location.NAME, cons.IMAGE, Row.QUANTITY, 0);
                    }
                    else
                    {
                        tableAssets.Rows.Add(false, Row.CID, cons.NAME, Row.LOCATIONID, location.NAME, cons.IMAGE, Row.QUANTITY, 0);
                    }
                }

                if (tableAssets.Rows.Count > 0)
                {
                    ListAssets.DataSource = tableAssets;
                    ListAssets.DataBind();
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
    }
}