﻿using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using RMS.Model;
using RMS.BLL;
using RMS.DAL;

namespace RMS.UI.SaleUI
{
    public partial class CreateSale : BasePage
    {
        private Product objProduct;
        private Sale objSale;
        private SaleDetail objSaleDetail;
        private ProductBiz objProductBiz;
        private SaleBiz objSaleBiz;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Convert.ToDecimal(txtQty.Text) <= Convert.ToDecimal(txtProductStock.Text))
            {
                pnlSaleDetail.Visible = true;

                if (Session["SaleDetail"] == null)
                {
                    DataTable dt = GetDataTable();
                    DataRow dr = dt.NewRow();
                    dr["ProductCode"] = Convert.ToInt16(txtProductCode.Text.Trim());
                    dr["ProductName"] = txtProductName.Text;
                    dr["SaleQuantity"] = Convert.ToDecimal(txtQty.Text);
                    dr["SalePrice"] = Convert.ToDecimal(Request.Form[txtProductPricePerUnit.UniqueID]);
                    dr["ProductVat"] = Convert.ToDecimal(txtVat.Text);
                    dr["VatAmount"] = (Convert.ToDecimal(txtVat.Text)*
                                       Convert.ToDecimal(Request.Form[txtProductPricePerUnit.UniqueID])/100*
                                       Convert.ToDecimal(txtQty.Text));
                    dr["TotalAmount"] = Convert.ToDecimal(Request.Form[txtProductPricePerUnit.UniqueID])*
                                        Convert.ToDecimal(txtQty.Text);

                    dt.Rows.Add(dr);

                    Session["SaleDetail"] = dt;
                    gvSellProduct.DataSource = dt;
                    gvSellProduct.DataBind();

                    ResetField();
                }
                else
                {
                    DataTable dt = (DataTable) Session["SaleDetail"];
                    DataRow[] d = dt.Select("ProductCode=" + txtProductCode.Text);
                    if (d.Length > 0)
                    {
                        int i;
                        for (i = 0; i < dt.Rows.Count; i++)
                        {
                            if (Convert.ToInt16(dt.Rows[i]["ProductCode"]) == Convert.ToInt32(txtProductCode.Text))
                            {
                                dt.Rows[i]["ProductCode"] = Convert.ToInt16(txtProductCode.Text.Trim());
                                dt.Rows[i]["ProductName"] = txtProductName.Text;
                                dt.Rows[i]["SaleQuantity"] = Convert.ToDouble(txtQty.Text);
                                dt.Rows[i]["SalePrice"] = Convert.ToDouble(Request.Form[txtProductPricePerUnit.UniqueID]);
                                dt.Rows[i]["ProductVat"] = Convert.ToDouble(txtVat.Text);
                                dt.Rows[i]["VatAmount"] =
                                    (Convert.ToDouble(txtVat.Text)*
                                     Convert.ToDouble(Request.Form[txtProductPricePerUnit.UniqueID])/100*
                                     Convert.ToDouble(txtQty.Text)).ToString();
                                dt.Rows[i]["TotalAmount"] =
                                    Convert.ToDouble(Request.Form[txtProductPricePerUnit.UniqueID])*
                                    Convert.ToDouble(txtQty.Text);
                            }
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["ProductCode"] = Convert.ToInt16(txtProductCode.Text.Trim());
                        dr["ProductName"] = txtProductName.Text;
                        dr["SaleQuantity"] = Convert.ToDouble(txtQty.Text);
                        dr["SalePrice"] = Convert.ToDouble(Request.Form[txtProductPricePerUnit.UniqueID]);
                        dr["ProductVat"] = Convert.ToDouble(txtVat.Text);
                        dr["VatAmount"] =
                            (Convert.ToDouble(txtVat.Text)*
                             Convert.ToDouble(Request.Form[txtProductPricePerUnit.UniqueID])/100*
                             Convert.ToDouble(txtQty.Text)).ToString();
                        dr["TotalAmount"] = Convert.ToDouble(Request.Form[txtProductPricePerUnit.UniqueID])*
                                            Convert.ToDouble(txtQty.Text);

                        dt.Rows.Add(dr);
                    }

                    Session["SaleDetail"] = dt;
                    gvSellProduct.DataSource = dt;
                    gvSellProduct.DataBind();

                    ResetField();
                }

                //AmountCalculation();

                txtProductCode.Attributes.Add("onfocus", "this.select()");
                txtProductCode.Focus();
            }
            else
            {
                MessageBox("Sell quantity is more than stock! Please correct");

                txtQty.Attributes.Add("onfocus", "this.select()");
                txtQty.Focus();
            }
        }

        private DataTable GetDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductCode", typeof(int));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("SaleQuantity", typeof(decimal));
            dt.Columns.Add("SalePrice", typeof(decimal));
            dt.Columns.Add("ProductVat", typeof(decimal));
            dt.Columns.Add("VatAmount", typeof(decimal));
            dt.Columns.Add("TotalAmount", typeof(decimal));

            return dt;
        }

        private void ResetField()
        {
            txtProductCode.Text = string.Empty;
            txtProductName.Text = string.Empty;
            txtProductStock.Text = string.Empty;
            txtProductPricePerUnit.Text = string.Empty;
            txtQty.Text = string.Empty;
            txtTotalPriceOfWholeQty.Text = string.Empty;
            txtVat.Text = string.Empty;
            txtTotalPriceOfWholeProduct.Text = string.Empty;
        }
        
        protected void btnSell_OnClick(object sender, EventArgs e)
        {
            objSale = new Sale();
            objSaleBiz = new SaleBiz();
            objSale.CustomerUsername = txtCustomerUsername.Text.Trim();
            objSale.TotalAmount = txtToBePaid.Text == string.Empty ? 0 : decimal.Parse(txtToBePaid.Text);
            objSale.DiscountAmount = txtDiscount.Text.Trim() == string.Empty ? 0 : decimal.Parse(txtDiscount.Text.Trim());
            objSale.PaidAmount = decimal.Parse(txtAmountPaid.Text);
            objSale.VatAmountTotal = decimal.Parse(txtVatAmountTotal.Text);
            objSale.PaymentMethod = Convert.ToByte(ddlPaymentMethod.SelectedValue);
            objSale.CreatedBy = Convert.ToInt16(Session["UserId"].ToString());
            objSale.Remarks = txtRemarks.Text.Trim();

            // Insert purchase details
            DataTable dt = (DataTable)Session["SaleDetail"];
            int i;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                objSale.ProductId = Convert.ToInt16(dt.Rows[i][0].ToString());
                objSale.SellQuantity = Convert.ToDecimal(dt.Rows[i][2].ToString());
                objSale.SellPrice = Convert.ToDecimal(dt.Rows[i][3].ToString());
                objSale.Vat = Convert.ToDecimal(dt.Rows[i][4].ToString());
                objSale.VatAmount = Convert.ToDecimal(dt.Rows[i][5].ToString());
                objSaleBiz.SellProduct(objSale);
                objSale.PaymentMethod = 0;
            }

            MessageBox("You are successfully sale listed products. Thanks!");

            Session.Remove("SaleDetail");

            Response.Redirect("CreateSale.aspx");
        }

        protected void gvSellProduct_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RowDelete")
            {
                GridViewRow gvr = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                int rowIndex = gvr.RowIndex;
                int id = Convert.ToInt32(((Label)gvr.FindControl("ProductCode")).Text);
                DataTable dt = (DataTable)Session["SaleDetail"];

                foreach (DataRow dataRow in dt.Rows)
                {
                    if (Convert.ToInt32(dataRow["ProductCode"]) == id)
                    {
                        dataRow.Delete();
                        break;
                    }
                }
                Session["SaleDetail"] = dt;
                gvSellProduct.DataSource = dt;
                gvSellProduct.DataBind();

                txtProductCode.Attributes.Add("onfocus", "this.select()");
                txtProductCode.Focus();
            }

            if (e.CommandName == "RowEdit")
            {
                GridViewRow gvr = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                int rowIndex = gvr.RowIndex;
                int id = Convert.ToInt32(((Label)gvr.FindControl("ProductCode")).Text);
                DataTable dt = (DataTable)Session["SaleDetail"];

                foreach (DataRow dataRow in dt.Rows)
                {
                    if (Convert.ToInt32(dataRow["ProductCode"]) == id)
                    {
                        objProductBiz = new ProductBiz();
                        objProduct = new Product();
                        objProduct = objProductBiz.AddProductForPurchase(id.ToString());

                        txtProductStock.Text = objProduct.ProductStock.ToString();
                        txtProductCode.Text = dataRow["ProductCode"].ToString();
                        txtProductName.Text = dataRow["ProductName"].ToString();
                        txtProductPricePerUnit.Text = dataRow["SalePrice"].ToString();
                        txtVat.Text = dataRow["ProductVat"].ToString();
                        txtQty.Text = dataRow["SaleQuantity"].ToString();
                        txtTotalPriceOfWholeQty.Text = dataRow["TotalAmount"].ToString();
                        txtTotalPriceOfWholeProduct.Text = (Convert.ToDecimal(dataRow["TotalAmount"]) + Convert.ToDecimal(dataRow["ProductVat"])).ToString();
                        break;
                    }
                }
                txtQty.Attributes.Add("onfocus", "this.select()");
                txtQty.Focus();
            }

            if (gvSellProduct.Rows.Count == 0)
            {
                txtProductCode.Attributes.Add("onfocus", "this.select()");
                txtProductCode.Focus();

                txtToBePaid.Text = string.Empty;
                pnlSaleDetail.Visible = false;
            }
        }

        private decimal totalAmount = 0;
        private decimal vatAmount = 0;
        protected void gvSellProduct_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Right;

                totalAmount += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "TotalAmount"));
                vatAmount += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "VatAmount"));
                
                txtToBePaid.Text = totalAmount.ToString("N2"); //Display amount in sale master Total Amount field

                txtVatAmountTotal.Text = vatAmount.ToString("N2"); //Display vat amount in sale master Vat Amount field

                txtTotalAmountWithVat.Text = (totalAmount + vatAmount).ToString("N2"); //Display total with vat amount in sale master
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[4].Text = "Total";
                e.Row.Cells[4].Font.Bold = true;
                e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Right;

                e.Row.Cells[5].Text = vatAmount.ToString("N2");
                e.Row.Cells[5].Font.Bold = true;
                e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Right;

                e.Row.Cells[6].Text = totalAmount.ToString("N2");
                e.Row.Cells[6].Font.Bold = true;
                e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;

                e.Row.Cells[7].Text = (totalAmount + vatAmount).ToString("N2");
                e.Row.Cells[7].Font.Bold = true;
                e.Row.Cells[7].ForeColor = Color.Red;
                e.Row.Cells[7].BackColor = Color.White;
                e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Center;
            }
        }
    }
}