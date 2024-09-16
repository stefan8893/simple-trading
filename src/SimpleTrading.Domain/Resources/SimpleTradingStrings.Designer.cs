﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SimpleTrading.Domain.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class SimpleTradingStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SimpleTradingStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SimpleTrading.Domain.Resources.SimpleTradingStrings", typeof(SimpleTradingStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Asset.
        /// </summary>
        public static string Asset {
            get {
                return ResourceManager.GetString("Asset", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bilanz.
        /// </summary>
        public static string Balance {
            get {
                return ResourceManager.GetString("Balance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Aktualisierung von &apos;Bilanz&apos; und &apos;Abgeschlossen&apos; ist nur möglich, wenn ein Trade bereits abgeschlossen wurde..
        /// </summary>
        public static string BalanceAndClosedUpdatesAreOnlyPossibleForClosedTrades {
            get {
                return ResourceManager.GetString("BalanceAndClosedUpdatesAreOnlyPossibleForClosedTrades", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Bilanz ist nicht 0, aber die Position deutet auf eine Bilanz gleich 0 hin..
        /// </summary>
        public static string BalanceNotZeroAndExitEntryPricesSame {
            get {
                return ResourceManager.GetString("BalanceNotZeroAndExitEntryPricesSame", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Bilanz ist 0, aber die Position deutet auf eine Bilanz ungleich 0 hin..
        /// </summary>
        public static string BalanceZeroAndExitEntryPricesNotTheSame {
            get {
                return ResourceManager.GetString("BalanceZeroAndExitEntryPricesNotTheSame", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kostendeckend.
        /// </summary>
        public static string BreakEven {
            get {
                return ResourceManager.GetString("BreakEven", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ihr Trade deutet auf das Ergebnis &apos;{0}&apos; hin, aber Sie haben &apos;{1}&apos; angegeben..
        /// </summary>
        public static string CalculatedAndManualResultMismatch {
            get {
                return ResourceManager.GetString("CalculatedAndManualResultMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ihre Position deutet auf das Ergebnis &apos;{0}&apos; hin, aber anhand der Bilanz ist es &apos;{1}&apos;..
        /// </summary>
        public static string CalculatedResultsMismatch {
            get {
                return ResourceManager.GetString("CalculatedResultsMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Abgeschlossen.
        /// </summary>
        public static string Closed {
            get {
                return ResourceManager.GetString("Closed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Abgeschlossen&apos; muss nach &apos;Eröffnet&apos; liegen..
        /// </summary>
        public static string ClosedBeforeOpened {
            get {
                return ResourceManager.GetString("ClosedBeforeOpened", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Abgeschlossen&apos; kann maximal nur einen Tag in der Zukunft liegen..
        /// </summary>
        public static string ClosedTooFarInTheFuture {
            get {
                return ResourceManager.GetString("ClosedTooFarInTheFuture", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Um einen abgeschlossenen Trade hinzuzufügen, müssen Sie &apos;Bilanz&apos; und &apos;Abgeschlossen&apos; angeben..
        /// </summary>
        public static string ClosedTradeNeedsClosedAndBalance {
            get {
                return ResourceManager.GetString("ClosedTradeNeedsClosedAndBalance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vergleichswert.
        /// </summary>
        public static string ComparisonValue {
            get {
                return ResourceManager.GetString("ComparisonValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Währung.
        /// </summary>
        public static string Currency {
            get {
                return ResourceManager.GetString("Currency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Einstiegspreis.
        /// </summary>
        public static string EntryPrice {
            get {
                return ResourceManager.GetString("EntryPrice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ausstiegspreis.
        /// </summary>
        public static string ExitPrice {
            get {
                return ResourceManager.GetString("ExitPrice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Feld.
        /// </summary>
        public static string Field {
            get {
                return ResourceManager.GetString("Field", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{PropertyValue}&apos; kann nicht als Filter benutzt werden..
        /// </summary>
        public static string FilterNotSupported {
            get {
                return ResourceManager.GetString("FilterNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ungültiges Filterformat..
        /// </summary>
        public static string InvalidFilterFormat {
            get {
                return ResourceManager.GetString("InvalidFilterFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ungültiger Link..
        /// </summary>
        public static string InvalidLink {
            get {
                return ResourceManager.GetString("InvalidLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Der Wert von &apos;{0}&apos; muss kleiner oder gleich &apos;{1}&apos; sein..
        /// </summary>
        public static string LessThanOrEqualValidatorMessage {
            get {
                return ResourceManager.GetString("LessThanOrEqualValidatorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Link.
        /// </summary>
        public static string Link {
            get {
                return ResourceManager.GetString("Link", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Bilanz ist positiv, aber Ihre Long-Position deutet auf einen Verlust hin..
        /// </summary>
        public static string LongPositionExitLessThanEntryAndPositiveBalance {
            get {
                return ResourceManager.GetString("LongPositionExitLessThanEntryAndPositiveBalance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Verlust.
        /// </summary>
        public static string Loss {
            get {
                return ResourceManager.GetString("Loss", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mittelmäßig.
        /// </summary>
        public static string Mediocre {
            get {
                return ResourceManager.GetString("Mediocre", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sie können nicht mehr als {0} Referenzen zu einem Trade hinzufügen..
        /// </summary>
        public static string MoreThanXReferencesNotAllowed {
            get {
                return ResourceManager.GetString("MoreThanXReferencesNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Anmerkungen.
        /// </summary>
        public static string Notes {
            get {
                return ResourceManager.GetString("Notes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ressource nicht gefunden..
        /// </summary>
        public static string NotFound {
            get {
                return ResourceManager.GetString("NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} nicht gefunden..
        /// </summary>
        public static string NotFoundNamed {
            get {
                return ResourceManager.GetString("NotFoundNamed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Null ist hier nicht erlaubt..
        /// </summary>
        public static string NullNotAllowed {
            get {
                return ResourceManager.GetString("NullNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Eröffnet.
        /// </summary>
        public static string Opened {
            get {
                return ResourceManager.GetString("Opened", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operator.
        /// </summary>
        public static string Operator {
            get {
                return ResourceManager.GetString("Operator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Der Operator &apos;{PropertyValue}&apos; wird nicht unterstützt..
        /// </summary>
        public static string OperatorNotSupported {
            get {
                return ResourceManager.GetString("OperatorNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Seite.
        /// </summary>
        public static string Page {
            get {
                return ResourceManager.GetString("Page", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Einträge pro Seite.
        /// </summary>
        public static string PageSize {
            get {
                return ResourceManager.GetString("PageSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Profil.
        /// </summary>
        public static string Profile {
            get {
                return ResourceManager.GetString("Profile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Referenz.
        /// </summary>
        public static string Reference {
            get {
                return ResourceManager.GetString("Reference", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Referenztyp.
        /// </summary>
        public static string ReferenceType {
            get {
                return ResourceManager.GetString("ReferenceType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ergebnis.
        /// </summary>
        public static string Result {
            get {
                return ResourceManager.GetString("Result", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sie können das Ergebnis eines offenen Trades nicht zurücksetzen. Bitte schließen die den Trade vorher..
        /// </summary>
        public static string ResultOfAnOpenedTradeCannotBeReset {
            get {
                return ResourceManager.GetString("ResultOfAnOpenedTradeCannotBeReset", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Suchbegriff.
        /// </summary>
        public static string SearchTerm {
            get {
                return ResourceManager.GetString("SearchTerm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Bilanz ist positiv, aber Ihre Short-Position deutet auf einen Verlust hin..
        /// </summary>
        public static string ShortPositionExitGreaterThanEntryAndPositiveBalance {
            get {
                return ResourceManager.GetString("ShortPositionExitGreaterThanEntryAndPositiveBalance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sortierung.
        /// </summary>
        public static string Sort {
            get {
                return ResourceManager.GetString("Sort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Sortierung anhand von &apos;{PropertyValue}&apos; ist nicht möglich..
        /// </summary>
        public static string SortingNotSupported {
            get {
                return ResourceManager.GetString("SortingNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stop-Loss.
        /// </summary>
        public static string StopLoss {
            get {
                return ResourceManager.GetString("StopLoss", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Take-Profit.
        /// </summary>
        public static string TakeProfit {
            get {
                return ResourceManager.GetString("TakeProfit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Trade.
        /// </summary>
        public static string Trade {
            get {
                return ResourceManager.GetString("Trade", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Handelsvolumen.
        /// </summary>
        public static string TradeSize {
            get {
                return ResourceManager.GetString("TradeSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{PropertyValue}&apos; ist nicht zulässig..
        /// </summary>
        public static string ValueNotAllowed {
            get {
                return ResourceManager.GetString("ValueNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gewinn.
        /// </summary>
        public static string Win {
            get {
                return ResourceManager.GetString("Win", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; darf nicht leer sein, wenn &apos;{1}&apos; angegeben ist..
        /// </summary>
        public static string XMustNotBeEmptyIfYIsThere {
            get {
                return ResourceManager.GetString("XMustNotBeEmptyIfYIsThere", resourceCulture);
            }
        }
    }
}
