using System;
using Smobiler.Core.Controls;
using SMOSEC.CommLib;
using Smobiler.Device;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmConsumablesDetail : Smobiler.Core.Controls.MobileForm
    {
        #region ����
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������
        public string CID;  //�Ĳı��
        #endregion

        /// <summary>
        /// �鿴�ĲĿ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuant_Press(object sender, EventArgs e)
        {
            try
            {
                frmConQuant conQuant = new frmConQuant {CID = CID};
                Show(conQuant);
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// �༭�Ĳ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Press(object sender, EventArgs e)
        {
            try
            {
                frmConsumablesDetailEdit conEdit = new frmConsumablesDetailEdit() { CID = CID };
                Form.Show(conEdit, (MobileForm sender1, object args) =>
                {
                    if (conEdit.ShowResult == ShowResult.Yes)
                    {
                        ShowResult = ShowResult.Yes;
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
        /// �����˼����رյ�ǰ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConsumablesDetail_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
            {
                Close();
            }
                
        }

        /// <summary>
        /// �����ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConsumablesDetail_Load(object sender, EventArgs e)
        {
            try
            {
                Bind();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Bind()
        {
            try
            {
                var consumables = _autofacConfig.ConsumablesService.GetConsumablesByID(CID);
                txtAssID.Text = consumables.CID;
                txtCeiling.Text = consumables.SAFECEILING.ToString();
                txtFloor.Text = consumables.SAFEFLOOR.ToString();
                txtName.Text = consumables.NAME;
                txtNote.Text = consumables.NOTE;
                txtSPQ.Text = consumables.SPQ.ToString();
                txtSpe.Text = consumables.SPECIFICATION;
                txtUnit.Text = consumables.UNIT;
                ImgPicture.ResourceID = consumables.IMAGE;
                if (Client.Session["Role"].ToString() == "SMOSECUser")
                {
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// �Ĳ������ӡ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Press(object sender, EventArgs e)
        {
            try
            {
                PosPrinterEntityCollection Commands = new PosPrinterEntityCollection();
                Commands.Add(new PosPrinterProtocolEntity(PosPrinterProtocol.Initial));
                Commands.Add(new PosPrinterProtocolEntity(PosPrinterProtocol.EnabledBarcode));
                Commands.Add(new PosPrinterProtocolEntity(PosPrinterProtocol.AbsoluteLocation));
                Commands.Add(new PosPrinterBarcodeEntity(PosBarcodeType.CODE128Height, "62"));
                Commands.Add(new PosPrinterBarcodeEntity(PosBarcodeType.CODE128, CID));
                Commands.Add(new PosPrinterProtocolEntity(PosPrinterProtocol.DisabledBarcode));
                Commands.Add(new PosPrinterContentEntity(System.Environment.NewLine));
                Commands.Add(new PosPrinterProtocolEntity(PosPrinterProtocol.Cut));

                posPrinter1.Print(Commands, (obj, args) =>
                {
                    if (args.isError == true)
                        this.Toast("Error: " + args.error);
                    else
                        this.Toast("��ӡ�ɹ�");
                });
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ɾ���Ĳ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Press(object sender, EventArgs e)
        {
            try
            {
                ReturnInfo returnInfo = _autofacConfig.ConsumablesService.DeleteConsumables(CID);
                if (returnInfo.IsSuccess)
                {
                    ShowResult = ShowResult.Yes;
                    Toast("ɾ���ɹ�.");
                    Close();
                }
                else
                {
                    Toast(returnInfo.ErrorInfo);
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
    }
}