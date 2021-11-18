using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PX.Common;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN.Overrides.INDocumentRelease;

using LocationStatus = PX.Objects.IN.Overrides.INDocumentRelease.LocationStatus;
using SiteStatus = PX.Objects.IN.Overrides.INDocumentRelease.SiteStatus;

namespace PX.Objects.IN
{
	public class InventorySummaryEnqPlan6C : PXGraphExtension<InventorySummaryEnq>
    {
        private bool timestampSelected = false;
        public virtual String ControlTimeStamp()
        {
            if (!timestampSelected)
            {
                PXDatabase.SelectTimeStamp();
                timestampSelected = true;
            }
            InventorySummaryEnq.Definition defs = PX.Common.PXContext.GetSlot<InventorySummaryEnq.Definition>();
            if (defs == null)
            {
                PXContext.SetSlot<InventorySummaryEnq.Definition>(
                defs = PXDatabase.GetSlot<InventorySummaryEnq.Definition>(nameof(InventorySummaryEnq) + "$ControlTimeStampDefinition",
                  new Type[] /// <see cref="iSERecordsFetch"/> for proper tables
            {
              typeof(InventoryItem),
              typeof(INSubItem),
              typeof(INItemPlan),

              typeof(INSite),
              typeof(INSiteStatus),
              typeof(INSiteCostStatus),

              typeof(INLocation),
              typeof(INLocationStatus),
              typeof(INLocationCostStatus),

              typeof(INLotSerClass),
              typeof(INLotSerialStatus),
              typeof(INItemLotSerial),
              typeof(INCostStatus),
              typeof(INCostSubItemXRef),
                  })
                );
            }
            return defs.TimeStamp;
        }

        protected virtual void AppendFiltersExt<T>(PXSelectBase<T> cmd, InventorySummaryEnqFilter filter)
          where T : class, IBqlTable, new()
        {
            if (filter.InventoryID != null)
            {
                cmd.WhereAnd<Where<InventoryItem.inventoryID, Equal<Current<InventorySummaryEnqFilter.inventoryID>>>>();
            }

            if (!SubCDUtils.IsSubCDEmpty(filter.SubItemCD))
            {
                cmd.WhereAnd<Where<INSubItem.subItemCD, Like<Current<InventorySummaryEnqFilter.subItemCDWildcard>>>>();
            }

            if (filter.SiteID != null)
            {
                cmd.WhereAnd<Where<INSite.siteID, Equal<Current<InventorySummaryEnqFilter.siteID>>>>();
            }

            if (typeof(T) != typeof(SiteStatus) && typeof(T) != typeof(INItemPlan) && filter.LocationID != null)
            {
                cmd.WhereAnd<Where<INLocation.locationID, Equal<Current<InventorySummaryEnqFilter.locationID>>>>();
            }
        }

        private decimal Round(decimal? a)
        {
            return Math.Round(a ?? 0m, (int)Base.commonsetup.Current.DecPlQty, MidpointRounding.AwayFromZero);
        }

        private decimal RoundUnitCost(decimal? a)
        {
            return Math.Round(a ?? 0m, (int)Base.commonsetup.Current.DecPlPrcCst, MidpointRounding.AwayFromZero);
        }

        private void Add(IStatus dest, IStatus source)
        {
            dest.QtyAvail += source.QtyAvail ?? 0m;
            dest.QtyHardAvail += source.QtyHardAvail ?? 0m;
            dest.QtyActual += source.QtyActual ?? 0m;
            dest.QtyNotAvail += source.QtyNotAvail ?? 0m;
            dest.QtyExpired += source.QtyExpired ?? 0m;
            dest.QtyOnHand += source.QtyOnHand ?? 0m;

            dest.QtyFSSrvOrdPrepared += source.QtyFSSrvOrdPrepared ?? 0m;
            dest.QtyFSSrvOrdBooked += source.QtyFSSrvOrdBooked ?? 0m;
            dest.QtyFSSrvOrdAllocated += source.QtyFSSrvOrdAllocated ?? 0m;

            dest.QtySOPrepared += source.QtySOPrepared ?? 0m;
            dest.QtySOBooked += source.QtySOBooked ?? 0m;
            dest.QtySOShipping += source.QtySOShipping ?? 0m;
            dest.QtySOShipped += source.QtySOShipped ?? 0m;
            dest.QtySOBackOrdered += source.QtySOBackOrdered ?? 0m;
            dest.QtyINAssemblyDemand += source.QtyINAssemblyDemand ?? 0m;
            dest.QtyINAssemblySupply += source.QtyINAssemblySupply ?? 0m;
            dest.QtyInTransitToProduction += source.QtyInTransitToProduction ?? 0m;
            dest.QtyProductionSupplyPrepared += source.QtyProductionSupplyPrepared ?? 0m;
            dest.QtyProductionSupply += source.QtyProductionSupply ?? 0m;
            dest.QtyPOFixedProductionPrepared += source.QtyPOFixedProductionPrepared ?? 0m;
            dest.QtyPOFixedProductionOrders += source.QtyPOFixedProductionOrders ?? 0m;
            dest.QtyProductionDemandPrepared += source.QtyProductionDemandPrepared ?? 0m;
            dest.QtyProductionDemand += source.QtyProductionDemand ?? 0m;
            dest.QtyProductionAllocated += source.QtyProductionAllocated ?? 0m;
            dest.QtySOFixedProduction += source.QtySOFixedProduction ?? 0m;
            dest.QtyProdFixedPurchase += source.QtyProdFixedPurchase ?? 0m;
            dest.QtyProdFixedProduction += source.QtyProdFixedProduction ?? 0m;
            dest.QtyProdFixedProdOrdersPrepared += source.QtyProdFixedProdOrdersPrepared ?? 0m;
            dest.QtyProdFixedProdOrders += source.QtyProdFixedProdOrders ?? 0m;
            dest.QtyProdFixedSalesOrdersPrepared += source.QtyProdFixedSalesOrdersPrepared ?? 0m;
            dest.QtyProdFixedSalesOrders += source.QtyProdFixedSalesOrders ?? 0m;
            dest.QtyINIssues += source.QtyINIssues ?? 0m;
            dest.QtyINReceipts += source.QtyINReceipts ?? 0m;
            dest.QtyInTransit += source.QtyInTransit ?? 0m;
            dest.QtyInTransitToSO += source.QtyInTransitToSO ?? 0m;
            dest.QtyPOPrepared += source.QtyPOPrepared ?? 0m;
            dest.QtyPOOrders += source.QtyPOOrders ?? 0m;
            dest.QtyPOReceipts += source.QtyPOReceipts ?? 0m;

            dest.QtyFixedFSSrvOrd += source.QtyFixedFSSrvOrd ?? 0m;
            dest.QtyPOFixedFSSrvOrd += source.QtyPOFixedFSSrvOrd ?? 0m;
            dest.QtyPOFixedFSSrvOrdPrepared += source.QtyPOFixedFSSrvOrdPrepared ?? 0m;
            dest.QtyPOFixedFSSrvOrdReceipts += source.QtyPOFixedFSSrvOrdReceipts ?? 0m;

            dest.QtySOFixed += source.QtySOFixed ?? 0m;
            dest.QtyPOFixedOrders += source.QtyPOFixedOrders ?? 0m;
            dest.QtyPOFixedPrepared += source.QtyPOFixedPrepared ?? 0m;
            dest.QtyPOFixedReceipts += source.QtyPOFixedReceipts ?? 0m;
            dest.QtySODropShip += source.QtySODropShip ?? 0m;
            dest.QtyPODropShipOrders += source.QtyPODropShipOrders ?? 0m;
            dest.QtyPODropShipPrepared += source.QtyPODropShipPrepared ?? 0m;
            dest.QtyPODropShipReceipts += source.QtyPODropShipReceipts ?? 0m;

            ICostStatus costdest = dest as ICostStatus;
            ICostStatus costsource = source as ICostStatus;
            if (costdest != null && costsource != null)
            {
                costdest.TotalCost += costsource.TotalCost;
            }
        }

        private void Subtract(IStatus dest, IStatus source)
        {
            dest.QtyAvail -= source.QtyAvail ?? 0m;
            dest.QtyHardAvail -= source.QtyHardAvail ?? 0m;
            dest.QtyActual -= source.QtyActual ?? 0m;
            dest.QtyNotAvail -= source.QtyNotAvail ?? 0m;
            dest.QtyExpired -= source.QtyExpired ?? 0m;
            dest.QtyOnHand -= source.QtyOnHand ?? 0m;

            dest.QtyFSSrvOrdPrepared -= source.QtyFSSrvOrdPrepared ?? 0m;
            dest.QtyFSSrvOrdBooked -= source.QtyFSSrvOrdBooked ?? 0m;
            dest.QtyFSSrvOrdAllocated -= source.QtyFSSrvOrdAllocated ?? 0m;

            dest.QtySOPrepared -= source.QtySOPrepared ?? 0m;
            dest.QtySOBooked -= source.QtySOBooked ?? 0m;
            dest.QtySOShipping -= source.QtySOShipping ?? 0m;
            dest.QtySOShipped -= source.QtySOShipped ?? 0m;
            dest.QtySOBackOrdered -= source.QtySOBackOrdered ?? 0m;
            dest.QtyINAssemblyDemand -= source.QtyINAssemblyDemand ?? 0m;
            dest.QtyINAssemblySupply -= source.QtyINAssemblySupply ?? 0m;
            dest.QtyInTransitToProduction -= source.QtyInTransitToProduction ?? 0m;
            dest.QtyProductionSupplyPrepared -= source.QtyProductionSupplyPrepared ?? 0m;
            dest.QtyProductionSupply -= source.QtyProductionSupply ?? 0m;
            dest.QtyPOFixedProductionPrepared -= source.QtyPOFixedProductionPrepared ?? 0m;
            dest.QtyPOFixedProductionOrders -= source.QtyPOFixedProductionOrders ?? 0m;
            dest.QtyProductionDemandPrepared -= source.QtyProductionDemandPrepared ?? 0m;
            dest.QtyProductionDemand -= source.QtyProductionDemand ?? 0m;
            dest.QtyProductionAllocated -= source.QtyProductionAllocated ?? 0m;
            dest.QtySOFixedProduction -= source.QtySOFixedProduction ?? 0m;
            dest.QtyProdFixedPurchase -= source.QtyProdFixedPurchase ?? 0m;
            dest.QtyProdFixedProduction -= source.QtyProdFixedProduction ?? 0m;
            dest.QtyProdFixedProdOrdersPrepared -= source.QtyProdFixedProdOrdersPrepared ?? 0m;
            dest.QtyProdFixedProdOrders -= source.QtyProdFixedProdOrders ?? 0m;
            dest.QtyProdFixedSalesOrdersPrepared -= source.QtyProdFixedSalesOrdersPrepared ?? 0m;
            dest.QtyProdFixedSalesOrders -= source.QtyProdFixedSalesOrders ?? 0m;
            dest.QtyINIssues -= source.QtyINIssues ?? 0m;
            dest.QtyINReceipts -= source.QtyINReceipts ?? 0m;
            dest.QtyInTransit -= source.QtyInTransit ?? 0m;
            dest.QtyInTransitToSO -= source.QtyInTransitToSO ?? 0m;
            dest.QtyPOPrepared -= source.QtyPOPrepared ?? 0m;
            dest.QtyPOOrders -= source.QtyPOOrders ?? 0m;
            dest.QtyPOReceipts -= source.QtyPOReceipts ?? 0m;

            dest.QtyFixedFSSrvOrd -= source.QtyFixedFSSrvOrd ?? 0m;
            dest.QtyPOFixedFSSrvOrd -= source.QtyPOFixedFSSrvOrd ?? 0m;
            dest.QtyPOFixedFSSrvOrdPrepared -= source.QtyPOFixedFSSrvOrdPrepared ?? 0m;
            dest.QtyPOFixedFSSrvOrdReceipts -= source.QtyPOFixedFSSrvOrdReceipts ?? 0m;

            dest.QtySOFixed -= source.QtySOFixed ?? 0m;
            dest.QtyPOFixedOrders -= source.QtyPOFixedOrders ?? 0m;
            dest.QtyPOFixedPrepared -= source.QtyPOFixedPrepared ?? 0m;
            dest.QtyPOFixedReceipts -= source.QtyPOFixedReceipts ?? 0m;
            dest.QtySODropShip -= source.QtySODropShip ?? 0m;
            dest.QtyPODropShipOrders -= source.QtyPODropShipOrders ?? 0m;
            dest.QtyPODropShipPrepared -= source.QtyPODropShipPrepared ?? 0m;
            dest.QtyPODropShipReceipts -= source.QtyPODropShipReceipts ?? 0m;

            ICostStatus costdest = dest as ICostStatus;
            ICostStatus costsource = source as ICostStatus;
            if (costdest != null && costsource != null)
            {
                costdest.TotalCost -= costsource.TotalCost;
            }
        }

        private void Copy(IStatus dest, IStatus source)
        {
            dest.QtyAvail = source.QtyAvail ?? 0m;
            dest.QtyHardAvail = source.QtyHardAvail ?? 0m;
            dest.QtyActual = source.QtyActual ?? 0m;
            dest.QtyNotAvail = source.QtyNotAvail ?? 0m;
            dest.QtyOnHand = source.QtyOnHand ?? 0m;

            dest.QtyFSSrvOrdPrepared = source.QtyFSSrvOrdPrepared ?? 0m;
            dest.QtyFSSrvOrdBooked = source.QtyFSSrvOrdBooked ?? 0m;
            dest.QtyFSSrvOrdAllocated = source.QtyFSSrvOrdAllocated ?? 0m;

            dest.QtySOPrepared = source.QtySOPrepared ?? 0m;
            dest.QtySOBooked = source.QtySOBooked ?? 0m;
            dest.QtySOShipping = source.QtySOShipping ?? 0m;
            dest.QtySOShipped = source.QtySOShipped ?? 0m;
            dest.QtySOBackOrdered = source.QtySOBackOrdered ?? 0m;
            dest.QtyINAssemblyDemand = source.QtyINAssemblyDemand ?? 0m;
            dest.QtyINAssemblySupply = source.QtyINAssemblySupply ?? 0m;
            dest.QtyInTransitToProduction = source.QtyInTransitToProduction ?? 0m;
            dest.QtyProductionSupplyPrepared = source.QtyProductionSupplyPrepared ?? 0m;
            dest.QtyProductionSupply = source.QtyProductionSupply ?? 0m;
            dest.QtyPOFixedProductionPrepared = source.QtyPOFixedProductionPrepared ?? 0m;
            dest.QtyPOFixedProductionOrders = source.QtyPOFixedProductionOrders ?? 0m;
            dest.QtyProductionDemandPrepared = source.QtyProductionDemandPrepared ?? 0m;
            dest.QtyProductionDemand = source.QtyProductionDemand ?? 0m;
            dest.QtyProductionAllocated = source.QtyProductionAllocated ?? 0m;
            dest.QtySOFixedProduction = source.QtySOFixedProduction ?? 0m;
            dest.QtyProdFixedPurchase = source.QtyProdFixedPurchase ?? 0m;
            dest.QtyProdFixedProduction = source.QtyProdFixedProduction ?? 0m;
            dest.QtyProdFixedProdOrdersPrepared = source.QtyProdFixedProdOrdersPrepared ?? 0m;
            dest.QtyProdFixedProdOrders = source.QtyProdFixedProdOrders ?? 0m;
            dest.QtyProdFixedSalesOrdersPrepared = source.QtyProdFixedSalesOrdersPrepared ?? 0m;
            dest.QtyProdFixedSalesOrders = source.QtyProdFixedSalesOrders ?? 0m;
            dest.QtyINIssues = source.QtyINIssues ?? 0m;
            dest.QtyINReceipts = source.QtyINReceipts ?? 0m;
            dest.QtyInTransit = source.QtyInTransit ?? 0m;
            dest.QtyInTransitToSO = source.QtyInTransitToSO ?? 0m;
            dest.QtyPOPrepared = source.QtyPOPrepared ?? 0m;
            dest.QtyPOOrders = source.QtyPOOrders ?? 0m;
            dest.QtyPOReceipts = source.QtyPOReceipts ?? 0m;

            dest.QtyFixedFSSrvOrd = source.QtyFixedFSSrvOrd ?? 0m;
            dest.QtyPOFixedFSSrvOrd = source.QtyPOFixedFSSrvOrd ?? 0m;
            dest.QtyPOFixedFSSrvOrdPrepared = source.QtyPOFixedFSSrvOrdPrepared ?? 0m;
            dest.QtyPOFixedFSSrvOrdReceipts = source.QtyPOFixedFSSrvOrdReceipts ?? 0m;

            dest.QtySOFixed = source.QtySOFixed ?? 0m;
            dest.QtyPOFixedOrders = source.QtyPOFixedOrders ?? 0m;
            dest.QtyPOFixedPrepared = source.QtyPOFixedPrepared ?? 0m;
            dest.QtyPOFixedReceipts = source.QtyPOFixedReceipts ?? 0m;
            dest.QtySODropShip = source.QtySODropShip ?? 0m;
            dest.QtyPODropShipOrders = source.QtyPODropShipOrders ?? 0m;
            dest.QtyPODropShipPrepared = source.QtyPODropShipPrepared ?? 0m;
            dest.QtyPODropShipReceipts = source.QtyPODropShipReceipts ?? 0m;
        }

        private T Copy<T>(IStatus source)
          where T : class, IStatus, new()
        {
            T dest = new T();
            Copy(dest, source);
            return dest;
        }

        private T Subtract<T>(IStatus a, IStatus b)
          where T : class, IBqlTable, IStatus, new()
        {
            T c = new T();
            if (typeof(T).IsAssignableFrom(a.GetType()))
            {
                PXCache<T>.RestoreCopy(c, (T)a);
                Subtract(c, b);
                return c;
            }

            c.QtyAvail = Round((a.QtyAvail ?? 0m) - (b.QtyAvail ?? 0m));
            c.QtyHardAvail = Round((a.QtyHardAvail ?? 0m) - (b.QtyHardAvail ?? 0m));
            c.QtyActual = Round((a.QtyActual ?? 0m) - (b.QtyActual ?? 0m));
            c.QtyNotAvail = Round((a.QtyNotAvail ?? 0m) - (b.QtyNotAvail ?? 0m));
            c.QtyExpired = Round((a.QtyExpired ?? 0m) - (b.QtyExpired ?? 0m));
            c.QtyOnHand = Round((a.QtyOnHand ?? 0m) - (b.QtyOnHand ?? 0m));

            c.QtyFSSrvOrdPrepared = Round((a.QtyFSSrvOrdPrepared ?? 0m) - (b.QtyFSSrvOrdPrepared ?? 0m));
            c.QtyFSSrvOrdBooked = Round((a.QtyFSSrvOrdBooked ?? 0m) - (b.QtyFSSrvOrdBooked ?? 0m));
            c.QtyFSSrvOrdAllocated = Round((a.QtyFSSrvOrdAllocated ?? 0m) - (b.QtyFSSrvOrdAllocated ?? 0m));

            c.QtySOPrepared = Round((a.QtySOPrepared ?? 0m) - (b.QtySOPrepared ?? 0m));
            c.QtySOBooked = Round((a.QtySOBooked ?? 0m) - (b.QtySOBooked ?? 0m));
            c.QtySOShipping = Round((a.QtySOShipping ?? 0m) - (b.QtySOShipping ?? 0m));
            c.QtySOShipped = Round((a.QtySOShipped ?? 0m) - (b.QtySOShipped ?? 0m));
            c.QtySOBackOrdered = Round((a.QtySOBackOrdered ?? 0m) - (b.QtySOBackOrdered ?? 0m));
            c.QtyINIssues = Round((a.QtyINIssues ?? 0m) - (b.QtyINIssues ?? 0m));
            c.QtyINReceipts = Round((a.QtyINReceipts ?? 0m) - (b.QtyINReceipts ?? 0m));
            c.QtyInTransit = Round((a.QtyInTransit ?? 0m) - (b.QtyInTransit ?? 0m));
            c.QtyInTransitToSO = Round((a.QtyInTransitToSO ?? 0m) - (b.QtyInTransitToSO ?? 0m));
            c.QtyINAssemblyDemand = Round((a.QtyINAssemblyDemand ?? 0m) - (b.QtyINAssemblyDemand ?? 0m));
            c.QtyINAssemblySupply = Round((a.QtyINAssemblySupply ?? 0m) - (b.QtyINAssemblySupply ?? 0m));
            c.QtyInTransitToProduction = Round((a.QtyInTransitToProduction ?? 0m) - (b.QtyInTransitToProduction ?? 0m));
            c.QtyProductionSupplyPrepared = Round((a.QtyProductionSupplyPrepared ?? 0m) - (b.QtyProductionSupplyPrepared ?? 0m));
            c.QtyProductionSupply = Round((a.QtyProductionSupply ?? 0m) - (b.QtyProductionSupply ?? 0m));
            c.QtyPOFixedProductionPrepared = Round((a.QtyPOFixedProductionPrepared ?? 0m) - (b.QtyPOFixedProductionPrepared ?? 0m));
            c.QtyPOFixedProductionOrders = Round((a.QtyPOFixedProductionOrders ?? 0m) - (b.QtyPOFixedProductionOrders ?? 0m));
            c.QtyProductionDemandPrepared = Round((a.QtyProductionDemandPrepared ?? 0m) - (b.QtyProductionDemandPrepared ?? 0m));
            c.QtyProductionDemand = Round((a.QtyProductionDemand ?? 0m) - (b.QtyProductionDemand ?? 0m));
            c.QtyProductionAllocated = Round((a.QtyProductionAllocated ?? 0m) - (b.QtyProductionAllocated ?? 0m));
            c.QtySOFixedProduction = Round((a.QtySOFixedProduction ?? 0m) - (b.QtySOFixedProduction ?? 0m));
            c.QtyProdFixedPurchase = Round((a.QtyProdFixedPurchase ?? 0m) - (b.QtyProdFixedPurchase ?? 0m));
            c.QtyProdFixedProduction = Round((a.QtyProdFixedProduction ?? 0m) - (b.QtyProdFixedProduction ?? 0m));
            c.QtyProdFixedProdOrdersPrepared = Round((a.QtyProdFixedProdOrdersPrepared ?? 0m) - (b.QtyProdFixedProdOrdersPrepared ?? 0m));
            c.QtyProdFixedProdOrders = Round((a.QtyProdFixedProdOrders ?? 0m) - (b.QtyProdFixedProdOrders ?? 0m));
            c.QtyProdFixedSalesOrdersPrepared = Round((a.QtyProdFixedSalesOrdersPrepared ?? 0m) - (b.QtyProdFixedSalesOrdersPrepared ?? 0m));
            c.QtyProdFixedSalesOrders = Round((a.QtyProdFixedSalesOrders ?? 0m) - (b.QtyProdFixedSalesOrders ?? 0m));
            c.QtyPOPrepared = Round((a.QtyPOPrepared ?? 0m) - (b.QtyPOPrepared ?? 0m));
            c.QtyPOOrders = Round((a.QtyPOOrders ?? 0m) - (b.QtyPOOrders ?? 0m));
            c.QtyPOReceipts = Round((a.QtyPOReceipts ?? 0m) - (b.QtyPOReceipts ?? 0m));

            c.QtyFixedFSSrvOrd = Round((a.QtyFixedFSSrvOrd ?? 0m) - (b.QtyFixedFSSrvOrd ?? 0m));
            c.QtyPOFixedFSSrvOrd = Round((a.QtyPOFixedFSSrvOrd ?? 0m) - (b.QtyPOFixedFSSrvOrd ?? 0m));
            c.QtyPOFixedFSSrvOrdPrepared = Round((a.QtyPOFixedFSSrvOrdPrepared ?? 0m) - (b.QtyPOFixedFSSrvOrdPrepared ?? 0m));
            c.QtyPOFixedFSSrvOrdReceipts = Round((a.QtyPOFixedFSSrvOrdReceipts ?? 0m) - (b.QtyPOFixedFSSrvOrdReceipts ?? 0m));

            c.QtySOFixed = Round((a.QtySOFixed ?? 0m) - (b.QtySOFixed ?? 0m));
            c.QtyPOFixedOrders = Round((a.QtyPOFixedOrders ?? 0m) - (b.QtyPOFixedOrders ?? 0m));
            c.QtyPOFixedPrepared = Round((a.QtyPOFixedPrepared ?? 0m) - (b.QtyPOFixedPrepared ?? 0m));
            c.QtyPOFixedReceipts = Round((a.QtyPOFixedReceipts ?? 0m) - (b.QtyPOFixedReceipts ?? 0m));
            c.QtySODropShip = Round((a.QtySODropShip ?? 0m) - (b.QtySODropShip ?? 0m));
            c.QtyPODropShipOrders = Round((a.QtyPODropShipOrders ?? 0m) - (b.QtyPODropShipOrders ?? 0m));
            c.QtyPODropShipPrepared = Round((a.QtyPODropShipPrepared ?? 0m) - (b.QtyPODropShipPrepared ?? 0m));
            c.QtyPODropShipReceipts = Round((a.QtyPODropShipReceipts ?? 0m) - (b.QtyPODropShipReceipts ?? 0m));

            ICostStatus cost_a = a as ICostStatus;
            ICostStatus cost_b = b as ICostStatus;
            ICostStatus cost_c = c as ICostStatus;
            if (cost_a != null && cost_b != null && cost_c != null)
            {
                cost_c.TotalCost = (cost_a.TotalCost ?? 0m) - (cost_b.TotalCost ?? 0m);
            }

            return c;
        }

        private T Add<T>(IStatus a, IStatus b)
          where T : class, IBqlTable, IStatus, new()
        {
            T c = new T();
            if (typeof(T).IsAssignableFrom(a.GetType()))
            {
                PXCache<T>.RestoreCopy(c, (T)a);
                Add(c, b);
                return c;
            }

            c.QtyAvail = Round((a.QtyAvail ?? 0m) + (b.QtyAvail ?? 0m));
            c.QtyHardAvail = Round((a.QtyHardAvail ?? 0m) + (b.QtyHardAvail ?? 0m));
            c.QtyActual = Round((a.QtyActual ?? 0m) + (b.QtyActual ?? 0m));
            c.QtyNotAvail = Round((a.QtyNotAvail ?? 0m) + (b.QtyNotAvail ?? 0m));
            c.QtyExpired = Round((a.QtyExpired ?? 0m) + (b.QtyExpired ?? 0m));

            c.QtyFSSrvOrdPrepared = Round((a.QtyFSSrvOrdPrepared ?? 0m) + (b.QtyFSSrvOrdPrepared ?? 0m));
            c.QtyFSSrvOrdBooked = Round((a.QtyFSSrvOrdBooked ?? 0m) + (b.QtyFSSrvOrdBooked ?? 0m));
            c.QtyFSSrvOrdAllocated = Round((a.QtyFSSrvOrdAllocated ?? 0m) + (b.QtyFSSrvOrdAllocated ?? 0m));

            c.QtyOnHand = Round((a.QtyOnHand ?? 0m) + (b.QtyOnHand ?? 0m));
            c.QtySOPrepared = Round((a.QtySOPrepared ?? 0m) + (b.QtySOPrepared ?? 0m));
            c.QtySOBooked = Round((a.QtySOBooked ?? 0m) + (b.QtySOBooked ?? 0m));
            c.QtySOShipping = Round((a.QtySOShipping ?? 0m) + (b.QtySOShipping ?? 0m));
            c.QtySOShipped = Round((a.QtySOShipped ?? 0m) + (b.QtySOShipped ?? 0m));
            c.QtySOBackOrdered = Round((a.QtySOBackOrdered ?? 0m) + (b.QtySOBackOrdered ?? 0m));
            c.QtyINIssues = Round((a.QtyINIssues ?? 0m) + (b.QtyINIssues ?? 0m));
            c.QtyINReceipts = Round((a.QtyINReceipts ?? 0m) + (b.QtyINReceipts ?? 0m));
            c.QtyInTransit = Round((a.QtyInTransit ?? 0m) + (b.QtyInTransit ?? 0m));
            c.QtyInTransitToSO = Round((a.QtyInTransitToSO ?? 0m) + (b.QtyInTransitToSO ?? 0m));
            c.QtyINAssemblyDemand = Round((a.QtyINAssemblyDemand ?? 0m) + (b.QtyINAssemblyDemand ?? 0m));
            c.QtyINAssemblySupply = Round((a.QtyINAssemblySupply ?? 0m) + (b.QtyINAssemblySupply ?? 0m));
            c.QtyInTransitToProduction = Round((a.QtyInTransitToProduction ?? 0m) + (b.QtyInTransitToProduction ?? 0m));
            c.QtyProductionSupplyPrepared = Round((a.QtyProductionSupplyPrepared ?? 0m) + (b.QtyProductionSupplyPrepared ?? 0m));
            c.QtyProductionSupply = Round((a.QtyProductionSupply ?? 0m) + (b.QtyProductionSupply ?? 0m));
            c.QtyPOFixedProductionPrepared = Round((a.QtyPOFixedProductionPrepared ?? 0m) + (b.QtyPOFixedProductionPrepared ?? 0m));
            c.QtyPOFixedProductionOrders = Round((a.QtyPOFixedProductionOrders ?? 0m) + (b.QtyPOFixedProductionOrders ?? 0m));
            c.QtyProductionDemandPrepared = Round((a.QtyProductionDemandPrepared ?? 0m) + (b.QtyProductionDemandPrepared ?? 0m));
            c.QtyProductionDemand = Round((a.QtyProductionDemand ?? 0m) + (b.QtyProductionDemand ?? 0m));
            c.QtyProductionAllocated = Round((a.QtyProductionAllocated ?? 0m) + (b.QtyProductionAllocated ?? 0m));
            c.QtySOFixedProduction = Round((a.QtySOFixedProduction ?? 0m) + (b.QtySOFixedProduction ?? 0m));
            c.QtyProdFixedPurchase = Round((a.QtyProdFixedPurchase ?? 0m) + (b.QtyProdFixedPurchase ?? 0m));
            c.QtyProdFixedProduction = Round((a.QtyProdFixedProduction ?? 0m) + (b.QtyProdFixedProduction ?? 0m));
            c.QtyProdFixedProdOrdersPrepared = Round((a.QtyProdFixedProdOrdersPrepared ?? 0m) + (b.QtyProdFixedProdOrdersPrepared ?? 0m));
            c.QtyProdFixedProdOrders = Round((a.QtyProdFixedProdOrders ?? 0m) + (b.QtyProdFixedProdOrders ?? 0m));
            c.QtyProdFixedSalesOrdersPrepared = Round((a.QtyProdFixedSalesOrdersPrepared ?? 0m) + (b.QtyProdFixedSalesOrdersPrepared ?? 0m));
            c.QtyProdFixedSalesOrders = Round((a.QtyProdFixedSalesOrders ?? 0m) + (b.QtyProdFixedSalesOrders ?? 0m));
            c.QtyPOPrepared = Round((a.QtyPOPrepared ?? 0m) + (b.QtyPOPrepared ?? 0m));
            c.QtyPOOrders = Round((a.QtyPOOrders ?? 0m) + (b.QtyPOOrders ?? 0m));
            c.QtyPOReceipts = Round((a.QtyPOReceipts ?? 0m) + (b.QtyPOReceipts ?? 0m));

            c.QtyFixedFSSrvOrd = Round((a.QtyFixedFSSrvOrd ?? 0m) + (b.QtyFixedFSSrvOrd ?? 0m));
            c.QtyPOFixedFSSrvOrd = Round((a.QtyPOFixedFSSrvOrd ?? 0m) + (b.QtyPOFixedFSSrvOrd ?? 0m));
            c.QtyPOFixedFSSrvOrdPrepared = Round((a.QtyPOFixedFSSrvOrdPrepared ?? 0m) + (b.QtyPOFixedFSSrvOrdPrepared ?? 0m));
            c.QtyPOFixedFSSrvOrdReceipts = Round((a.QtyPOFixedFSSrvOrdReceipts ?? 0m) + (b.QtyPOFixedFSSrvOrdReceipts ?? 0m));

            c.QtySOFixed = Round((a.QtySOFixed ?? 0m) + (b.QtySOFixed ?? 0m));
            c.QtyPOFixedOrders = Round((a.QtyPOFixedOrders ?? 0m) + (b.QtyPOFixedOrders ?? 0m));
            c.QtyPOFixedPrepared = Round((a.QtyPOFixedPrepared ?? 0m) + (b.QtyPOFixedPrepared ?? 0m));
            c.QtyPOFixedReceipts = Round((a.QtyPOFixedReceipts ?? 0m) + (b.QtyPOFixedReceipts ?? 0m));
            c.QtySODropShip = Round((a.QtySODropShip ?? 0m) + (b.QtySODropShip ?? 0m));
            c.QtyPODropShipOrders = Round((a.QtyPODropShipOrders ?? 0m) + (b.QtyPODropShipOrders ?? 0m));
            c.QtyPODropShipPrepared = Round((a.QtyPODropShipPrepared ?? 0m) + (b.QtyPODropShipPrepared ?? 0m));
            c.QtyPODropShipReceipts = Round((a.QtyPODropShipReceipts ?? 0m) + (b.QtyPODropShipReceipts ?? 0m));

            ICostStatus cost_a = a as ICostStatus;
            ICostStatus cost_b = b as ICostStatus;
            ICostStatus cost_c = c as ICostStatus;
            if (cost_a != null && cost_b != null && cost_c != null)
            {
                cost_c.TotalCost = (cost_a.TotalCost ?? 0m) + (cost_b.TotalCost ?? 0m);
            }

            return c;
        }

        public delegate IEnumerable iSERecordsFetchDel();
        [PXOverride]
        public virtual IEnumerable iSERecordsFetch(iSERecordsFetchDel baseM)
        {
            {
                var resultset = new List<(InventorySummaryEnquiryResult Result, INSubItem SubItem, INSite Site, INLocation Location)>();

                int decPlQty = (Base.commonsetup.Current.DecPlQty ?? 6);
                int decPlPrcCst = (Base.commonsetup.Current.DecPlPrcCst ?? 6);

                InventorySummaryEnqFilter filter = Base.Filter.Current;

                Base.Caches[typeof(INSiteStatus)].Clear();
                Base.Caches[typeof(INLocationStatus)].Clear();

                Base.Caches[typeof(SiteStatus)].Clear();
                Base.Caches[typeof(LocationStatus)].Clear();

                PXSelectBase<INLotSerialStatus> cmd_lss = new PXSelectReadonly2<INLotSerialStatus,
                  InnerJoin<INLocation,
                    On<INLotSerialStatus.FK.Location>,
                  InnerJoin<InventoryItem,
                    On<InventoryItem.inventoryID, Equal<INLotSerialStatus.inventoryID>,
                    And<Match<InventoryItem, Current<AccessInfo.userName>>>>,
                  InnerJoin<INSite,
                    On2<INLotSerialStatus.FK.Site,
                    And<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>,
                    And<Match<INSite, Current<AccessInfo.userName>>>>>,
                  InnerJoin<INSubItem,
                    On<INLotSerialStatus.FK.SubItem>,
                  LeftJoin<INLotSerClass,
                    On<InventoryItem.FK.LotSerialClass>,
                  LeftJoin<INLocationCostStatus,
                    On<INLocationCostStatus.inventoryID, Equal<INLotSerialStatus.inventoryID>,
                      And<INLocationCostStatus.subItemID, Equal<INLotSerialStatus.subItemID>,
                      And<INLocationCostStatus.locationID, Equal<INLotSerialStatus.locationID>>>>,
                  LeftJoin<INSiteCostStatus,
                    On<INSiteCostStatus.inventoryID, Equal<INLotSerialStatus.inventoryID>,
                      And<INSiteCostStatus.subItemID, Equal<INLotSerialStatus.subItemID>,
                      And<INSiteCostStatus.siteID, Equal<INLotSerialStatus.siteID>>>>>>>>>>>,
                  Where<INLotSerialStatus.inventoryID, Equal<Current<InventorySummaryEnqFilter.inventoryID>>,
                    And<Where<Current<InventorySummaryEnqFilter.expandByLotSerialNbr>, Equal<True>, And<INLotSerClass.lotSerAssign, Equal<INLotSerAssign.whenReceived>,
                    Or<InventoryItem.valMethod, Equal<INValMethod.specific>>>>>>>(Base);

                AppendFiltersExt<INLotSerialStatus>(cmd_lss, filter);

                var cmdCostStatus = new PXSelectReadonly<INLotSerialCostStatus,
                  Where<INLotSerialCostStatus.inventoryID, Equal<Current<InventorySummaryEnqFilter.inventoryID>>,
                    And<INLotSerialCostStatus.lotSerialNbr, IsNotNull>>>(Base);

                if (filter.SiteID != null)
                    cmdCostStatus.WhereAnd<Where<INLotSerialCostStatus.siteID, Equal<Current<InventorySummaryEnqFilter.siteID>>>>();

                if (filter.InventoryID != null)
                    foreach (INLotSerialCostStatus res in cmdCostStatus.Select())
                        cmdCostStatus.Cache.SetStatus(res, PXEntryStatus.Notchanged);

                using (new PXFieldScope(cmd_lss.View,
                  typeof(INLotSerialStatus),
                  typeof(INSite.siteCD),
                  typeof(INSubItem.subItemCD),
                  typeof(INLocation.locationCD),
                  typeof(INLocation.inclQtyAvail),
                  typeof(InventoryItem.baseUnit),
                  typeof(INLocationCostStatus),
                  typeof(INSiteCostStatus)))
                {
                    foreach (PXResult<INLotSerialStatus, INLocation, InventoryItem, INSite, INSubItem> res in cmd_lss.Select())
                    {
                        INLotSerialStatus lss_rec = res;
                        INLocation loc_rec = res;
                        InventoryItem item_rec = res;

                        InventorySummaryEnquiryResult ret = Copy<InventorySummaryEnquiryResult>(lss_rec);
                        ret.InventoryID = lss_rec.InventoryID;
                        ret.SubItemID = lss_rec.SubItemID;
                        ret.SiteID = lss_rec.SiteID;
                        ret.LocationID = lss_rec.LocationID;
                        ret.LotSerialNbr = lss_rec.LotSerialNbr;
                        ret.ExpireDate = lss_rec.ExpireDate;
                        ret.BaseUnit = item_rec.BaseUnit;
                        ret.TotalCost = 0m;
                        ret.QtyExpired = 0m;
                        ret.ControlTimetamp = ControlTimeStamp();

                        INLotSerialCostStatus lscs_rec = (INLotSerialCostStatus)cmdCostStatus.Cache.Locate(new INLotSerialCostStatus()
                        {
                            InventoryID = lss_rec.InventoryID,
                            SubItemID = lss_rec.SubItemID,
                            SiteID = lss_rec.SiteID,
                            LotSerialNbr = lss_rec.LotSerialNbr
                        });

                        INLocationCostStatus lcs_rec = PXResult.Unwrap<INLocationCostStatus>(res);
                        INSiteCostStatus scs_rec = PXResult.Unwrap<INSiteCostStatus>(res);

                        if (lscs_rec?.InventoryID != null)
                        {
                            ret.UnitCost = lscs_rec.UnitCost;
                            ret.TotalCost = PXDBCurrencyAttribute.BaseRound(Base, (decimal)(ret.QtyOnHand * lscs_rec.UnitCost));
                        }
                        else if (lcs_rec.InventoryID != null)
                        {
                            ret.UnitCost = lcs_rec.UnitCost;
                            ret.TotalCost = PXDBCurrencyAttribute.BaseRound(Base, (decimal)(ret.QtyOnHand * lcs_rec.UnitCost));
                        }
                        else if (scs_rec.InventoryID != null)
                        {
                            ret.UnitCost = scs_rec.UnitCost;
                            ret.TotalCost = PXDBCurrencyAttribute.BaseRound(Base, (decimal)(ret.QtyOnHand * scs_rec.UnitCost));
                        }

                        if (loc_rec.InclQtyAvail == false)
                        {
                            ret.QtyNotAvail = ret.QtyAvail;
                            ret.QtyAvail = 0m;
                            ret.QtyHardAvail = 0m;
                            ret.QtyActual = 0m;
                        }
                        else
                        {
                            ret.QtyNotAvail = 0m;
                        }

                        ret.ExpireDate = lss_rec.ExpireDate;
                        if (lss_rec.ExpireDate != null && DateTime.Compare((DateTime)Base.Accessinfo.BusinessDate, (DateTime)lss_rec.ExpireDate) > 0)
                        {
                            ret.QtyExpired = lss_rec.QtyOnHand;
                        }

                        if (PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>())
                        {
                            PXCache cache = Base.Caches[typeof(LocationStatus)];
                            LocationStatus aggregate = new LocationStatus();
                            aggregate.InventoryID = ret.InventoryID;
                            aggregate.SubItemID = ret.SubItemID;
                            aggregate.SiteID = ret.SiteID;
                            aggregate.LocationID = ret.LocationID;

                            aggregate = (LocationStatus)cache.Insert(aggregate);
                            Add(aggregate, ret);
                        }
                        else
                        {
                            PXCache cache = Base.Caches[typeof(SiteStatus)];
                            SiteStatus aggregate = new SiteStatus();
                            aggregate.InventoryID = ret.InventoryID;
                            aggregate.SubItemID = ret.SubItemID;
                            aggregate.SiteID = ret.SiteID;

                            aggregate = (SiteStatus)cache.Insert(aggregate);
                            Add(aggregate, ret);

                        }

                        if (filter.ExpandByLotSerialNbr == true)
                        {
                            ret.QtyAvail -= ret.QtyExpired;
                            if (!ret.IsZero()) resultset.Add((ret, res, res, loc_rec));
                        }
                    }
                }

                PXSelectBase<INLocationStatus> cmd_ls = new PXSelectReadonly2<INLocationStatus,
                  InnerJoin<INLocation,
                    On<INLocationStatus.FK.Location>,
                  InnerJoin<InventoryItem,
                    On<InventoryItem.inventoryID, Equal<INLocationStatus.inventoryID>,
                    And<Match<InventoryItem, Current<AccessInfo.userName>>>>,
                  InnerJoin<INSite,
                    On2<INLocationStatus.FK.Site,
                    And<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>,
                    And<Match<INSite, Current<AccessInfo.userName>>>>>,
                  InnerJoin<INSubItem,
                    On<INLocationStatus.FK.SubItem>,
                  LeftJoin<INLotSerClass,
                    On<InventoryItem.FK.LotSerialClass>,
                  LeftJoin<INLocationCostStatus,
                    On<INLocationCostStatus.inventoryID, Equal<INLocationStatus.inventoryID>,
                      And<INLocationCostStatus.subItemID, Equal<INLocationStatus.subItemID>,
                      And<INLocationCostStatus.locationID, Equal<INLocationStatus.locationID>>>>,
                  LeftJoin<INSiteCostStatus,
                    On<INSiteCostStatus.inventoryID, Equal<INLocationStatus.inventoryID>,
                      And<INSiteCostStatus.subItemID, Equal<INLocationStatus.subItemID>,
                      And<INSiteCostStatus.siteID, Equal<INLocationStatus.siteID>>>>>>>>>>>,
                  Where<INLocationStatus.inventoryID, Equal<Current<InventorySummaryEnqFilter.inventoryID>>>>(Base);

                AppendFiltersExt<INLocationStatus>(cmd_ls, filter);

                foreach (PXResult<INLocationStatus, INLocation, InventoryItem, INSite, INSubItem> res in cmd_ls.Select())
                {
                    INLocationStatus ls_rec = res;
                    INLocation loc_rec = res;
                    InventoryItem item_rec = res;

                    InventorySummaryEnquiryResult ret = Copy<InventorySummaryEnquiryResult>(ls_rec);
                    ret.InventoryID = ls_rec.InventoryID;
                    ret.SubItemID = ls_rec.SubItemID;
                    ret.SiteID = ls_rec.SiteID;
                    ret.LocationID = ls_rec.LocationID;
                    ret.BaseUnit = item_rec.BaseUnit;
                    ret.UnitCost = ret.UnitCost ?? 0m;
                    ret.TotalCost = 0m;
                    ret.QtyExpired = 0m;
                    ret.ControlTimetamp = ControlTimeStamp();

                    INLocationCostStatus lcs_rec = PXResult.Unwrap<INLocationCostStatus>(res);
                    INSiteCostStatus scs_rec = PXResult.Unwrap<INSiteCostStatus>(res);
                    if (lcs_rec.InventoryID != null)
                    {
                        ret.UnitCost = lcs_rec.UnitCost;
                        ret.TotalCost = PXDBCurrencyAttribute.BaseRound(Base, (decimal)(ret.QtyOnHand * lcs_rec.UnitCost));
                    }
                    else if (scs_rec.InventoryID != null)
                    {
                        ret.UnitCost = scs_rec.UnitCost;
                        ret.TotalCost = PXDBCurrencyAttribute.BaseRound(Base, (decimal)(ret.QtyOnHand * scs_rec.UnitCost));
                    }

                    if (loc_rec.InclQtyAvail == false)
                    {
                        ret.QtyNotAvail = ret.QtyAvail;
                        ret.QtyAvail = 0m;
                        ret.QtyHardAvail = 0m;
                        ret.QtyActual = 0m;
                    }
                    else
                    {
                        ret.QtyNotAvail = 0m;
                    }

                    if (PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>())
                    {
                        PXCache cache = Base.Caches[typeof(SiteStatus)];
                        SiteStatus aggregate = new SiteStatus();
                        aggregate.InventoryID = ret.InventoryID;
                        aggregate.SubItemID = ret.SubItemID;
                        aggregate.SiteID = ret.SiteID;

                        aggregate = (SiteStatus)cache.Insert(aggregate);
                        Add(aggregate, ret);

                    }

                    {
                        PXCache cache = Base.Caches[typeof(LocationStatus)];
                        LocationStatus aggregate = new LocationStatus();
                        aggregate.InventoryID = ret.InventoryID;
                        aggregate.SubItemID = ret.SubItemID;
                        aggregate.SiteID = ret.SiteID;
                        aggregate.LocationID = ret.LocationID;

                        aggregate = (LocationStatus)cache.Insert(aggregate);
                        if (filter.ExpandByLotSerialNbr == true)
                        {
                            ret = Subtract<InventorySummaryEnquiryResult>(ret, aggregate);

                            if (PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>())
                            {
                                if (!ret.IsZero()) resultset.Add((ret, res, res, loc_rec));
                            }
                            continue;
                        }
                        else if (aggregate.TotalCost != 0m)
                        {
                            ret.TotalCost = aggregate.TotalCost;
                            ret.UnitCost = RoundUnitCost(aggregate.QtyOnHand != 0m ? aggregate.TotalCost / aggregate.QtyOnHand : 0m);
                        }
                    }

                    if (PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>())
                    {
                        if (!ret.IsZero()) resultset.Add((ret, res, res, loc_rec));
                    }
                }


                if (filter.ExpandByLotSerialNbr == true)
                {
                    var shippingPlanTypes = PXSelectReadonly<INPlanType, Where<INPlanType.inclQtySOShipping, Equal<True>>>.SelectMultiBound(Base, null).AsEnumerable();
                    PXSelectBase<INItemPlan> cmd_plans = new PXSelectReadonly2<INItemPlan,
                    InnerJoin<InventoryItem,
                      On2<INItemPlan.FK.InventoryItem,
                      And<Match<InventoryItem, Current<AccessInfo.userName>>>>,
                    InnerJoin<INSite,
                      On2<INItemPlan.FK.Site,
                      And<Match<INSite, Current<AccessInfo.userName>>>>,
                    InnerJoin<INSubItem,
                      On<INItemPlan.FK.SubItem>,
                    InnerJoin<INPlanType,
                      On<INItemPlan.FK.PlanType>>>>>,
                    Where<
                    INPlanType.inclQtySOShipping, Equal<decimal1>,
                    And<INItemPlan.inventoryID, Equal<Current<InventorySummaryEnqFilter.inventoryID>>>>>(Base);

                    AppendFiltersExt<INItemPlan>(cmd_plans, filter);

                    List<PXResult<INItemPlan, InventoryItem, INSite, INSubItem>> lstPlans = new List<PXResult<INItemPlan, InventoryItem, INSite, INSubItem>>();

                    foreach (PXResult<INItemPlan, InventoryItem, INSite, INSubItem> res in cmd_plans.Select())
                    {
                        lstPlans.Add(res);
                    }

                    for (int i = 0; i < lstPlans.Count; i++)
                    {
                        INItemPlan plan_rec = lstPlans[i];

                        if (shippingPlanTypes.Any(x => ((INPlanType)x).PlanType == plan_rec.OrigPlanType))
                        {
                            for (int j = 0; j < lstPlans.Count; j++)
                            {
                                INItemPlan origplan_rec = lstPlans[j];

                                if (origplan_rec.PlanID == plan_rec.OrigPlanID
                                  ||
                                  (origplan_rec.RefNoteID == plan_rec.OrigNoteID &&
                                  origplan_rec.PlanType == plan_rec.OrigPlanType &&
                                  origplan_rec.Reverse == plan_rec.Reverse &&
                                  origplan_rec.SubItemID == plan_rec.SubItemID &&
                                  origplan_rec.SiteID == plan_rec.SiteID &&
                                  origplan_rec.LocationID == null &&
                                  origplan_rec.LotSerialNbr == plan_rec.LotSerialNbr))
                                {
                                    origplan_rec.PlanQty -= plan_rec.PlanQty;
                                    plan_rec.PlanQty = 0m;
                                    break;
                                }
                            }
                        }
                    }

                    foreach (PXResult<INItemPlan, InventoryItem, INSite, INSubItem> res in lstPlans)
                    {
                        INItemPlan plan_rec = res;
                        InventoryItem item_rec = res;

                        if (plan_rec.PlanQty == 0m || String.IsNullOrEmpty(plan_rec.LotSerialNbr) || plan_rec.LocationID != null)
                            continue;
                        var ss_rec = new INSiteStatus();
                        ss_rec.SiteID = plan_rec.SiteID;
                        ss_rec.InventoryID = item_rec.InventoryID;
                        ss_rec.SubItemID = plan_rec.SubItemID;

                        InventorySummaryEnquiryResult ret = Copy<InventorySummaryEnquiryResult>(ss_rec);
                        ret.LotSerialNbr = plan_rec.LotSerialNbr;
                        ret.InventoryID = ss_rec.InventoryID;
                        ret.SubItemID = ss_rec.SubItemID;
                        ret.SiteID = ss_rec.SiteID;
                        ret.BaseUnit = item_rec.BaseUnit;
                        ret.UnitCost = ret.UnitCost ?? 0m;
                        ret.TotalCost = 0m;
                        ret.QtyExpired = 0m;
                        ret.ControlTimetamp = ControlTimeStamp();

                        PXCache cache = Base.Caches[typeof(SiteStatus)];
                        SiteStatus aggregate = new SiteStatus();
                        aggregate.InventoryID = ret.InventoryID;
                        aggregate.SubItemID = ret.SubItemID;
                        aggregate.SiteID = ret.SiteID;

                        aggregate = (SiteStatus)cache.Insert(aggregate);
                        var origaggregate = (SiteStatus)cache.CreateCopy(aggregate);

                        INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<SiteStatus>(Base, aggregate, plan_rec, shippingPlanTypes.First(x => ((INPlanType)x).PlanType == plan_rec.PlanType), aggregate.InclQtyAvail.GetValueOrDefault(), plan_rec.Hold);

                        Add(ret, aggregate);
                        ret = Subtract<InventorySummaryEnquiryResult>(ret, origaggregate);

                        if (!ret.IsZero())
                        {
                            resultset.Add((ret, res, res, INLocation.PK.Find(Base, ret.LocationID)));
                        }

                    }
                }

                PXSelectBase<SiteStatus> cmd_ss = new PXSelectReadonly2<SiteStatus,
                  InnerJoin<InventoryItem,
                    On<InventoryItem.inventoryID, Equal<SiteStatus.inventoryID>,
                    And<Match<InventoryItem, Current<AccessInfo.userName>>>>,
                  InnerJoin<INSite,
                    On2<SiteStatus.FK.Site,
                    And<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>,
                    And<Match<INSite, Current<AccessInfo.userName>>>>>,
                  InnerJoin<INSubItem,
                    On<SiteStatus.FK.SubItem>>>>,
                  Where<INSiteStatus.inventoryID, Equal<Current<InventorySummaryEnqFilter.inventoryID>>>>(Base);

                AppendFiltersExt<SiteStatus>(cmd_ss, filter);

                foreach (PXResult<SiteStatus, InventoryItem, INSite, INSubItem> res in cmd_ss.Select())
                {
                    SiteStatus ss_rec = res;
                    InventoryItem item_rec = res;

                    InventorySummaryEnquiryResult ret = Copy<InventorySummaryEnquiryResult>(ss_rec);
                    ret.InventoryID = ss_rec.InventoryID;
                    ret.SubItemID = ss_rec.SubItemID;
                    ret.SiteID = ss_rec.SiteID;
                    ret.BaseUnit = item_rec.BaseUnit;
                    ret.UnitCost = ret.UnitCost ?? 0m;
                    ret.TotalCost = 0m;
                    ret.QtyExpired = 0m;
                    ret.ControlTimetamp = ControlTimeStamp();
                    InventorySummaryEnquiryResultExtPlan6C retext = Base.Caches[typeof(InventorySummaryEnquiryResult)].GetExtension<InventorySummaryEnquiryResultExtPlan6C>(ret);
                    SiteStatusExtPlan6C ss_recex = ss_rec.GetExtension<SiteStatusExtPlan6C>();
                    retext.UsrQtySOTransfer = ss_recex.UsrQtySOTransfer;

                    if (PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>() || filter.ExpandByLotSerialNbr == true)
                    {
                        PXCache cache = Base.Caches[typeof(SiteStatus)];
                        SiteStatus aggregate = new SiteStatus();
                        aggregate.InventoryID = ret.InventoryID;
                        aggregate.SubItemID = ret.SubItemID;
                        aggregate.SiteID = ret.SiteID;

                        aggregate = (SiteStatus)cache.Insert(aggregate);
                        SiteStatusExtPlan6C aggrex = aggregate.GetExtension<SiteStatusExtPlan6C>();
                        //InventorySummaryEnquiryResultExtSOTransfers retex = ret.GetExtension<InventorySummaryEnquiryResultExtSOTransfers>();
                        ret = Subtract<InventorySummaryEnquiryResult>(ret, aggregate);
                    }

                    if (!PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>() || filter.LocationID == null)
                    {
                        if (!ret.IsZero())
                        {
                            resultset.Add((ret, res, res, null));
                        }

                    }
                }

                return resultset
                  .OrderBy(x => x.SubItem?.SubItemCD)
                  .ThenBy(x => x.Site?.SiteCD)
                  .ThenBy(x => x.Location?.LocationCD)
                  .ThenBy(x => x.Result.LotSerialNbr)
                  .Select(x => x.Result);
            }
        }
    }

}