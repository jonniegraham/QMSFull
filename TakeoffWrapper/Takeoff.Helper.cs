using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using DataAccess;
using ModelWrapper;
using ProgressBar;
using WorksheetWrapper;

namespace TakeoffWrapper
{
    partial class Takeoff
    {
        private struct TAKEOFF
        {
            public struct COLUMN
            {
                public const int Sku = 1, Star = 1;
                public const int Notes = 2, Job = 2, Contractor = 2, Heading = 2;
                public const int FileNumber_VISIBLE = 6, Quantity = 6;
                public const int FileNumber_HIDDEN = 26, FirstName = 26, LastName = 26;
                // for testing
                public const int UserId = 26;
            }
            public struct ROW
            {
                public const int FirstName = 1;
                public const int LastName = 2;
                public const int FileNumber_HIDDEN = 4;
                public const int FileNumber_VISIBLE = 18, Job = 18;
                public const int Contractor = 19;
                public const int DataBegin = 23;

                // for testing
                public const int UserId = 30;
            }
        }

        private struct SOQ
        {
            public struct COLUMN
            {
                public const int Heading = 1;
                public const int Notes = 1;
                public const int Sku = 2;
                public const int Description = 3;
                public const int FactoredQuantity = 5;
                public const int Unit = 6;
                public const int Rate = 7;
                public const int Discount = 8;
                public const int NetRate = 9;
                public const int Amount = 10;
            }
        }

        private void IncrementProgressBar()
        {
            Application.Current.Dispatcher.Invoke(_progressBar.Increment);
            _progressIncrementsRemaining--;
        }

        private void CloseProgressBar()
        {
            Application.Current.Dispatcher.Invoke(_progressBar.Close);
        }

        private void SetUpTakeoff(string path, string sheet)
        {
            Worksheet = new Worksheet();
            Worksheet.Initialize(path, sheet);
        }

        private void SetUpProgressBar()
        {
            _progressBar = new Animation();
            _progressBar.Initialize("Generating Schedule of Quantities...");
            _progressBar.Loaded += async (sender, e) =>
            {
                // code to run once progress bar is loaded.
                await Task.Factory.StartNew(async () => { await DoBuildSoq(); });
                CloseProgressBar();
            };
        }

        private async Task DoBuildSoq()
        {
            IncrementProgressBar();
            var soq = Worksheet.New("SOQ");
            IncrementProgressBar();
            Worksheet.ShowUpdating = false;
            SetColumnHeaders(soq);
            IncrementProgressBar();
            var quote = await Data.Instance().Quotes().GetQuoteByFileNumberAsync(FileNumber);
            IncrementProgressBar();
            SetProjectInfoSection(soq, quote);
            IncrementProgressBar();
            SetSummarySection(soq);
            IncrementProgressBar();
            SetAlternativesOrOptionsSection(soq);
            IncrementProgressBar();
            SetQuoteInfoSection(soq, quote);
            IncrementProgressBar();
            SetNoteWellSection(soq);
            IncrementProgressBar();
            SetCaveatSection(soq);
            IncrementProgressBar();
            await TransferRowsAsync(TAKEOFF.ROW.DataBegin, soq);
            Save(true);
        }

        private static double GetDouble(string str, double defaultValue)
        {
            if (!double.TryParse(str, out var dlbe))
                dlbe = defaultValue;
            return dlbe;
        }

        private static void SetColumnHeaders(IWorksheet worksheet)
        {

            worksheet.Rows[1].Columns[7].Insert(Direction.Horizontal, new[] {
                "Selling Margin →",
                "=(SUM(Amount)-SUM(cost.amount))/SUM(Amount)",
                "=(SUM(Amount)-SUM(cost.amount))/SUM(Amount)",
                "=SUM(margin.amount)"
            });

            worksheet.Rows[2].Columns[1].Insert(Direction.Horizontal, new[] {
                "SOQ",
                "Description",
                "Code",
                "Product",
                "L Code",
                "Qty",
                "Unit",
                "Rate",
                "Disc",
                "Nett Rate",
                "Amount",
                "Cost Rate",
                "Cost Amount",
                "Selling Margin %",
                "Amount Margin %"
            });
            worksheet.Rows[1].Columns[4].Width = 0;
            worksheet.Rows[1].Columns[9].Width = 0;
            worksheet.Rows[1].Height = 18.75;
            worksheet.Rows[2].Height = 25.5;
            worksheet.Rows[2].Columns[1].Underline(26);
        }

        private void SetProjectInfoSection(IWorksheet worksheet, QuoteWrapper quote)
        {
            worksheet.Rows[16].Columns[2].Insert(Direction.Vertical, new[] {
                "SCHEDULE OF MATERIAL",
                "for",
                Job,
                "at"
            });

            worksheet.Rows[23].Columns[3].Insert(Direction.Vertical, new[] {
                "(This document is to provide a guideline only and not to form part of a",
                " Contract)"
            });

            worksheet.Rows[26].Columns[2].Insert(Direction.Vertical, new[] {
                "1. An estimate of materials. This is an ESTIMATE ONLY. While Mega Petone has made",
                "reasonable efforts to the accuracy of this estimate, customers are advised that",
                "building techniques do vary considerably and on site circumstances cannot be",
                "anticipated by Mega Petone.",
                "2. Estimated prices for materials do not include G.S.T, which will be added when invoicing.",
                "3. No responsibility is held for any error in calculation volume, takeoff or",
                "estimation by Mega Petone.",
                "4. If only part of this Estimate is going to be used, we have the right to amend any of the",
                "discounts that apply.",
                "5. Cartage WILL be charged at"
            });

            worksheet.Rows[36].Columns[5].Insert(Direction.Vertical, new[] {
                "Small Truck From - $47.82",
                "Nees HIAB From - $100"
            });

            worksheet.Rows[37].Columns[10].Insert(
                "Courtesy Trailer - No Charge"
            );

            worksheet.Rows[38].Columns[2].Insert(
                "6. All prices valid for 30 days, then are subject increased costs and availability."
            );

            worksheet.Rows[13].Columns[2].Value = "Builder:";
            worksheet.Rows[13].Columns[3].Value = Contractor;
            worksheet.Rows[13].Columns[2].SetBold(true, 2, 8);
            worksheet.Rows[13].Columns[2].Underline(2);
            worksheet.Rows[13].Columns[2].HorizontalAlignment = Alignment.Right;

            worksheet.Rows[20].Columns[2].Value = quote.Client.Contact.Address.Number + " " + quote.Client.Contact.Address.Street + " " + quote.Client.Contact.Address.StreetType;
            worksheet.Rows[21].Columns[2].Value = quote.Client.Contact.Address.Town;
        }

        private static void SetSummarySection(IWorksheet worksheet)
        {
            worksheet.Rows[39].Columns[1].Insert(Direction.Vertical, new[] {
                "SUMMARY",
                "STAGE ONE - TO FLOOR",
                "See Below for Concrete Option",
                "",
                "STAGE TWO - FRAMING",
                "See Below for Framing Option",
                "STAGE THREE - CLOSED IN",
                "STAGE FOUR - FINISHING"
            });

            worksheet.Rows[40].Columns[3].Insert(Direction.Vertical, new[] {
                "Concrete Work",
                "Ground Floor Framing & Flooring ........",
                "Decking..................................................",
                "Upper Floor Framing & Flooring ........",
                "Wall Framing Accessories & Bracing ........",
                "Roof Framing............................................",
                "Exterior Sheathing and Trim ....................",
                "Interior Lining and Trim ..........................",
                "Doors and Frames ................................",
                "",
                "=IF(ROUND(Nett.Total,0)=ROUND(Discount.Total,0),\"Sub Total\",\"Totals Don't Balance\")",
                "GST",
                "Total"
            });

            worksheet.Rows[51].Columns[4].Insert("=J51/Nett.Total");

            worksheet.Rows[40].Columns[10].Insert(Direction.Vertical, new[] {
                "=SUM(Slab)",
                "=SUM(Floor)",
                "=SUM(Decking)",
                "=SUM(First.Floor)",
                "=SUM(Wall)",
                "=SUM(Roof)",
                "=SUM(Exterior)",
                "=SUM(Interior)",
                "=SUM(Doors)",
                "",
                "=SUM(summary)",
                "=(SUM(summary)*gst_tax)",
                "=J50+J51"
            });

            worksheet.Rows[39].Columns[1].SetBold(true, 1, 1);
            worksheet.Rows[39].Columns[1].SetItalic(true, 1, 1);
            worksheet.Rows[40].Columns[1].SetItalic(true, 10, 13);
            worksheet.Rows[50].Columns[3].SetItalic(true, 8, 13);
            worksheet.Rows[39].Height = 25;
            worksheet.Rows[40].Height = 25;
            worksheet.Rows[43].Height = 25;
            worksheet.Rows[46].Height = 25;
            worksheet.Rows[47].Height = 25;
            worksheet.Rows[50].Height = 30;
            worksheet.Rows[39].Columns[1].Shade(10, 1);
            worksheet.Rows[50].Columns[3].HorizontalAlignment = Alignment.Right;
            worksheet.Rows[51].Columns[3].HorizontalAlignment = Alignment.Right;
            worksheet.Rows[52].Columns[3].HorizontalAlignment = Alignment.Right;
        }

        private static void SetAlternativesOrOptionsSection(IWorksheet worksheet)
        {
            worksheet.Rows[54].Columns[1].Insert(Direction.Vertical, new[] {
                "ALTERNATIVES or OPTIONS",
                "(Not Included in Summary)"
            });

            worksheet.Rows[55].Columns[3].Insert(Direction.Vertical, new[] {
                "Concrete Supply Option...................",
                "Boxing Option......",
                "Concrete Block Footings",
                "Wall Framing Option in Lieu if Pre Nail",
                "Roofing .....................................",
                "Skylite Option......",
                "Spouting................................................",
                "Brick Veneer.....................",
                "Hardware...............................................",
                "Exterior Door",
                "Sundry Items.......................",
                "Plumbing Items................",
                "Bathroom Items......"
            });

            worksheet.Rows[55].Columns[10].Insert(Direction.Vertical, new[] {
                "=SUM(Concrete)",
                "=SUM(Boxing)",
                "=SUM(Blocks)",
                "=SUM(framing)",
                "=SUM(roofing)",
                "=SUM(Skylites)",
                "=SUM(spouting)",
                "=SUM(Bricks)",
                "=SUM(hardware)",
                "=SUM(ext.door)",
                "=SUM(sundry)",
                "=SUM(plumbing)",
                "=SUM(Bathroom)"
            });

            worksheet.Rows[54].Columns[1].SetBold(true, 1, 1);
            worksheet.Rows[54].Columns[1].SetItalic(true, 1, 1);
            worksheet.Rows[55].Columns[1].SetBold(true, 1, 1);
            worksheet.Rows[55].Columns[1].SetItalic(true, 10, 13);
            worksheet.Rows[54].Height = 17;
            worksheet.Rows[54].Columns[1].Shade(10, 1);
        }

        private static void SetQuoteInfoSection(IWorksheet worksheet, QuoteWrapper quote)
        {
            worksheet.Rows[71].Columns[2].Insert(Direction.Vertical, new[] {
                "Name",
                "Address",
                "Phone",
                "File No.",
                "Branch",
                "Takeoff"
            });

            worksheet.Rows[71].Columns[3].Insert(Direction.Vertical, new[] {
                quote.Client.Name,
                quote.Client.Contact.Address.Number + " " + quote.Client.Contact.Address.Street + " " + quote.Client.Contact.Address.StreetType + ", " + quote.Client.Contact.Address.Town,
                quote.Client.Contact.Phone.CellPhone,
                quote.FileNumber
            });

            worksheet.Rows[70].Height = 17;
            worksheet.Rows[74].Height = 19.5;
            worksheet.Rows[70].Columns[1].Shade(10, 1);
            worksheet.Rows[71].Columns[2].HorizontalAlignment = Alignment.Right;
            worksheet.Rows[72].Columns[2].HorizontalAlignment = Alignment.Right;
            worksheet.Rows[73].Columns[2].HorizontalAlignment = Alignment.Right;
            worksheet.Rows[74].Columns[2].HorizontalAlignment = Alignment.Right;
            worksheet.Rows[75].Columns[2].HorizontalAlignment = Alignment.Right;
            worksheet.Rows[76].Columns[2].HorizontalAlignment = Alignment.Right;
        }

        private static void SetNoteWellSection(IWorksheet worksheet)
        {
            worksheet.Rows[78].Columns[1].Insert(Direction.Vertical, new[] {
                "NOTE WELL",
                "This Document is NOT be Used for CONTRACTURAL Purposes",
                "No Specification Provided",
                "No Construction Details Provided",
                "Details Assumed to Complete This Document",
                "This Document is Provided as a Guide for Material Rates Only",
                "Contractor Shall Verify Suitablilty of All Selected Products especially DURABILTY Compliancy - BRANZ Bulletin 328",
                "Contractor MUST Verify Suitablilty of All Selected Products for Exposure Zone"
            });

            worksheet.Rows[78].Columns[1].Shade(10, 1);
            worksheet.Rows[78].Columns[1].SetBold(true, 1, 1);
            worksheet.Rows[78].Columns[1].SetItalic(true, 1, 1);
            worksheet.Rows[78].Height = 17;
            worksheet.Rows[79].Columns[1].SetBold(true, 1, 7);
            worksheet.Rows[79].Columns[1].SetItalic(true, 1, 7);
        }

        private static void SetCaveatSection(IWorksheet worksheet)
        {
            worksheet.Rows[86].Columns[1].Insert(Direction.Vertical, new[] {
                "The True Extent and Nature of Addition & Alteration Work Can Only  be Ascertained By On-Site Inspection",
                "Preparation of this document was from Drawings Only - The Contractor MUST Check & Confirm ALL on Site",
                "Contractor MUST allow for making good to existing work were required",
                "Only Work to New Areas Measured."
            });

            worksheet.Rows[86].Height = 25;
            worksheet.Rows[86].Columns[1].Shade(10, 4);
            worksheet.Rows[86].Columns[1].SetBold(true, 1, 4);
            worksheet.Rows[86].Columns[1].SetItalic(true, 1, 4);
        }

        private async Task TransferRowsAsync(int takeoffRow, IWorksheet soq)
        {
            if (takeoffRow >= Worksheet.Rows.Last)
                return;

            var totalRows = Worksheet.Rows.Last - TAKEOFF.ROW.DataBegin;
            var totalProgressIncrements = totalRows / _progressIncrementsRemaining;

            if ((takeoffRow - TAKEOFF.ROW.DataBegin) % totalProgressIncrements == 0)
                Application.Current.Dispatcher.Invoke(() => { _progressBar.Increment(); });


            var sku = Convert.ToString(Worksheet.Rows[takeoffRow].Columns[TAKEOFF.COLUMN.Sku].Value);

            if (!string.IsNullOrEmpty(sku))
            {
                if (sku.Equals("*"))
                {
                    ProcessStarredRow(takeoffRow, soq);
                }
                else
                {
                    var product = await Data.Instance().Products().GetProductBySkuAsync(sku);
                    if (product == null)
                    {
                        ProcessInvalidProductSku(soq, takeoffRow);
                    }
                    else
                    {
                        ProcessProduct(takeoffRow, soq, product);
                    }
                }
            }
            await TransferRowsAsync(++takeoffRow, soq);
        }

        private static void ProcessInvalidProductSku(IWorksheet soq, int takeoffRow) { }

        private void ProcessStarredRow(int takeoffRow, IWorksheet soq)
        {
            var soqRow = soq.Rows.Last + 1;
            soq.Rows[soqRow].Columns[SOQ.COLUMN.Heading].Value = Worksheet.Rows[takeoffRow].Columns[2].Value;

            if (Worksheet.Rows[takeoffRow].Columns[TAKEOFF.COLUMN.Heading].IsBold)
                soq.Rows[soqRow].Columns[SOQ.COLUMN.Heading].SetBold(true, 1, 1);
            else
            {
                var firstNonBoldCharNum = Worksheet.Rows[takeoffRow].Columns[TAKEOFF.COLUMN.Heading].FirstNonBoldCharNum;
                if (firstNonBoldCharNum > 1)
                {
                    soq.Rows[soqRow].Columns[SOQ.COLUMN.Heading].SetBoldTo(firstNonBoldCharNum - 1);
                }
            }

            if (Worksheet.Rows[takeoffRow].Columns[TAKEOFF.COLUMN.Star].IsShaded)
                soq.Rows[soqRow].Columns[SOQ.COLUMN.Heading].Shade(10, 1);
        }

        private void ProcessProduct(int takeoffRow, IWorksheet soq, ProductWapper product)
        {
            var soqRow = soq.Rows.Last + 1;

            if (!product.Sku.Equals("9000099") && !product.Sku.Equals("9000999"))
            {
                soq.Rows[soqRow].Columns[SOQ.COLUMN.Sku].Value = product.Sku;
                soq.Rows[soqRow].Columns[SOQ.COLUMN.Description].Value = product.Description;
                soq.Rows[soqRow].Columns[SOQ.COLUMN.Rate].Value = Equals(product.Retail, 0.0) ? "" : $"{product.Retail:0.00}";

                var discount = GetDouble(product.Discount?.ToString(), 0.0);

                soq.Rows[soqRow].Columns[SOQ.COLUMN.Discount].Value = Equals(discount, 0.0) ? "" : $"{discount:0.00}";
                soq.Rows[soqRow].Columns[SOQ.COLUMN.NetRate].Value = "=G" + soqRow + "-H" + soqRow + "*G" + soqRow;
                soq.Rows[soqRow].Columns[SOQ.COLUMN.NetRate].NumberFormat = "0.00";
                soq.Rows[soqRow].Columns[SOQ.COLUMN.Amount].Value = "=E" + soqRow + "*I" + soqRow;
                soq.Rows[soqRow].Columns[SOQ.COLUMN.Amount].NumberFormat = "0.00";
            }

            soq.Rows[soqRow].Columns[SOQ.COLUMN.Notes].Value = Worksheet.Rows[takeoffRow].Columns[TAKEOFF.COLUMN.Notes].Value;

            double qty = GetDouble(Worksheet.Rows[takeoffRow].Columns[TAKEOFF.COLUMN.Quantity].Value?.ToString(), 1.0);
            var factoredQty = GetDouble((product.MattFactor * qty).ToString(CultureInfo.InvariantCulture), 1 * qty);

            var factoredQtyStr = $"{factoredQty:0.0}";

            soq.Rows[soqRow].Columns[SOQ.COLUMN.FactoredQuantity].Value = factoredQtyStr;
            soq.Rows[soqRow].Columns[SOQ.COLUMN.Unit].Value = product.SUnit;
        }
    }
}
