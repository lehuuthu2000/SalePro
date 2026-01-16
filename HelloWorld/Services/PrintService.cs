using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace helloworld.Services
{
    public class PrintService
    {
        private Order _order;
        private DataTable _detailsTable;
        private string _customerName;
        private string _employeeName;

        public void PrintInvoice(Order order, DataTable detailsTable, string customerName, string employeeName)
        {
            _order = order;
            _detailsTable = detailsTable;
            _customerName = customerName;
            _employeeName = employeeName;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintPage);
            
            // Setup paper size if needed (e.g. Receipt 80mm)
            // pd.DefaultPageSettings.PaperSize = new PaperSize("Receipt", 280, 600); // approx 80mm
            
            PrintPreviewDialog ppd = new PrintPreviewDialog();
            ppd.Document = pd;
            ppd.Width = 800;
            ppd.Height = 600;
            ppd.ShowDialog();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font font = new Font("Courier New", 10);
            Font headerFont = new Font("Courier New", 16, FontStyle.Bold);
            Font boldFont = new Font("Courier New", 10, FontStyle.Bold);
            
            float fontHeight = font.GetHeight();
            int startX = 10;
            int startY = 10;
            int offset = 20;

            // Header
            graphics.DrawString("SALE PRO STORE", headerFont, Brushes.Black, startX, startY);
            offset += 30;
            graphics.DrawString("Address: 123 ABC Street, XYZ City", font, Brushes.Black, startX, startY + offset);
            offset += (int)fontHeight + 5;
            graphics.DrawString("Phone: 0123456789", font, Brushes.Black, startX, startY + offset);
            offset += 30;

            graphics.DrawString("INVOICE", new Font("Courier New", 14, FontStyle.Bold), Brushes.Black, startX, startY + offset);
            offset += 25;

            graphics.DrawString($"Order Code: {_order.OrderCode}", font, Brushes.Black, startX, startY + offset);
            offset += (int)fontHeight + 5;
            graphics.DrawString($"Date: {DateTime.Now:dd/MM/yyyy HH:mm}", font, Brushes.Black, startX, startY + offset);
            offset += (int)fontHeight + 5;
            graphics.DrawString($"Customer: {_customerName}", font, Brushes.Black, startX, startY + offset);
            offset += (int)fontHeight + 5;
            graphics.DrawString($"Staff: {_employeeName}", font, Brushes.Black, startX, startY + offset);
            offset += 20;

            // Table Header
            // We assume standard A4 or large enough width for now.
            // Layout: Product (40%), Qty(10%), Price(25%), Total(25%)
            // Coords: X=10, X=250, X=300, X=400 (approx)
            
            int colProd = 10;
            int colQty = 350;
            int colPrice = 400;
            int colTotal = 550;

            graphics.DrawString("Product", boldFont, Brushes.Black, colProd, startY + offset);
            graphics.DrawString("Qty", boldFont, Brushes.Black, colQty, startY + offset);
            graphics.DrawString("Price", boldFont, Brushes.Black, colPrice, startY + offset);
            graphics.DrawString("Total", boldFont, Brushes.Black, colTotal, startY + offset);
            
            offset += (int)fontHeight + 5;
            graphics.DrawLine(Pens.Black, startX, startY + offset, 700, startY + offset);
            offset += 5;

            // Items
            foreach (DataRow row in _detailsTable.Rows)
            {
                string productName = row["Tên sản phẩm"]?.ToString() ?? "Unknown";
                string qty = row["Số lượng"]?.ToString() ?? "0";
                decimal price = Convert.ToDecimal(row["Đơn giá"]);
                decimal itemTotal = Convert.ToDecimal(row["Thành tiền"]);

                // Wrap product name if too long?
                if (productName.Length > 30) productName = productName.Substring(0, 27) + "...";

                graphics.DrawString(productName, font, Brushes.Black, colProd, startY + offset);
                graphics.DrawString(qty, font, Brushes.Black, colQty, startY + offset);
                graphics.DrawString(price.ToString("N0"), font, Brushes.Black, colPrice, startY + offset);
                graphics.DrawString(itemTotal.ToString("N0"), font, Brushes.Black, colTotal, startY + offset);
                
                offset += (int)fontHeight + 5;
            }

            graphics.DrawLine(Pens.Black, startX, startY + offset, 700, startY + offset);
            offset += 10;

            // Totals
            graphics.DrawString($"Subtotal:", boldFont, Brushes.Black, colPrice, startY + offset);
            graphics.DrawString((_order.TotalAmount + _order.DiscountAmount - _order.Tax).ToString("N0"), font, Brushes.Black, colTotal, startY + offset);
            offset += (int)fontHeight + 5;

            if (_order.DiscountAmount > 0)
            {
                graphics.DrawString($"Discount:", boldFont, Brushes.Black, colPrice, startY + offset);
                graphics.DrawString("-" + _order.DiscountAmount.ToString("N0"), font, Brushes.Black, colTotal, startY + offset);
                offset += (int)fontHeight + 5;
            }

             if (_order.Tax > 0)
            {
                graphics.DrawString($"Tax:", boldFont, Brushes.Black, colPrice, startY + offset);
                graphics.DrawString(_order.Tax.ToString("N0"), font, Brushes.Black, colTotal, startY + offset);
                offset += (int)fontHeight + 5;
            }

            offset += 5;
            graphics.DrawString($"TOTAL:", new Font("Courier New", 12, FontStyle.Bold), Brushes.Black, colPrice, startY + offset);
            graphics.DrawString(_order.TotalAmount.ToString("N0"), new Font("Courier New", 12, FontStyle.Bold), Brushes.Black, colTotal, startY + offset);
            offset += 40;

            // Footer
            string footer = "Thank you for your business!";
            SizeF footerSize = graphics.MeasureString(footer, font);
            graphics.DrawString(footer, font, Brushes.Black, (700 - footerSize.Width) / 2, startY + offset);
        }
    }
}
