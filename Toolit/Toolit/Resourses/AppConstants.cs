using System.Collections.Generic;
using System.Collections.ObjectModel;
using Toolit.Models.Layouts;

namespace Toolit.Resourses
{
    public static class AppConstants
    {
        public static List<CraftLayoutModel> CraftModels => new List<CraftLayoutModel>()
        {
            new CraftLayoutModel()
            {
                ServerId = "Electrician",
                Code = 1,
                LocalName = AppResources.ElectricianLocalName,
                LocalDescription = AppResources.ElectricianLocalDescription,
                AddTaskIcon = "task_craft_electrician_icon",
                FilterIcon = "screwdriver_fade",
                LocalFullDescription = AppResources.ElectricianLocalFullDescription,
                LocalJobDescriptionPlaceholder = AppResources.ElectricianLocalJobDescriptionPlaceholder,
                Tags = new ObservableCollection<TagLayoutModel>()
                {
                    new TagLayoutModel() { ServerId = "ElectricianPowerOutlet",                LocalName = AppResources.ElectricianPowerOutletLocalName },
                    new TagLayoutModel() { ServerId = "ElectricianDimmer",                     LocalName = AppResources.ElectricianDimmerLocalName },
                    new TagLayoutModel() { ServerId = "ElectricianLampSwitch",                 LocalName = AppResources.ElectricianLampSwitchLocalName },
                    new TagLayoutModel() { ServerId = "ElectricianSpotlights",                 LocalName = AppResources.ElectricianSpotlightsLocalName },
                    new TagLayoutModel() { ServerId = "ElectricianLedStrip",                   LocalName = AppResources.ElectricianLedStripLocalName },
                    new TagLayoutModel() { ServerId = "ElectricianLamp",                       LocalName = AppResources.ElectricianLampLocalName },
                    new TagLayoutModel() { ServerId = "ElectricianWiring",                     LocalName = AppResources.ElectricianWiringLocalName },
                    new TagLayoutModel() { ServerId = "ElectricianElectricalPanel",            LocalName = AppResources.ElectricianElectricalPanelLocalName },
                    new TagLayoutModel() { ServerId = "ElectricianFittings",                   LocalName = AppResources.ElectricianFittingsLocalName },
                    new TagLayoutModel() { ServerId = "ElectricianGroundFaultCircuitBreaker",  LocalName = AppResources.ElectricianGroundFaultCircuitBreakerLocalName }
                }
            },
            new CraftLayoutModel()
            {
                ServerId = "Carpenter",
                Code = 2,
                LocalName = AppResources.CarpenterLocalName,
                LocalDescription = AppResources.CarpenterLocalDescription,
                AddTaskIcon = "hammer_green",
                FilterIcon = "screwdriver_fade",
                LocalFullDescription = AppResources.CarpenterLocalFullDescription,
                LocalJobDescriptionPlaceholder = AppResources.CarpenterLocalJobDescriptionPlaceholder,
                Tags = new ObservableCollection<TagLayoutModel>()
                {
                    new TagLayoutModel() {ServerId = "CarpenterWall",               LocalName = AppResources.CarpenterWallLocalName},
                    new TagLayoutModel() {ServerId = "CarpenterStucco",             LocalName = AppResources.CarpenterStuccoLocalName},
                    new TagLayoutModel() {ServerId = "CarpenterDoor",               LocalName = AppResources.CarpenterDoorLocalName},
                    new TagLayoutModel() {ServerId = "CarpenterDoorWithFrame",      LocalName = AppResources.CarpenterDoorWithFrameLocalName},
                    new TagLayoutModel() {ServerId = "CarpenterFloorMolding",       LocalName = AppResources.CarpenterFloorMoldingLocalName},
                    new TagLayoutModel() {ServerId = "CarpenterKitchen",            LocalName = AppResources.CarpenterKitchenLocalName},
                    new TagLayoutModel() {ServerId = "CarpenterWardrobe",           LocalName = AppResources.CarpenterWardrobeLocalName},
                    new TagLayoutModel() {ServerId = "CarpenterShelf",              LocalName = AppResources.CarpenterShelfLocalName}
                }
            },
            new CraftLayoutModel()
            {
                ServerId = "Painter",
                Code = 3,
                LocalName = AppResources.PainterLocalName,
                LocalDescription = AppResources.PainterLocalDescription,
                AddTaskIcon = "paint_brush_green",
                FilterIcon = "screwdriver_fade",
                LocalFullDescription = AppResources.PainterLocalFullDescription,
                LocalJobDescriptionPlaceholder = AppResources.PainterLocalJobDescriptionPlaceholder,
                Tags = new ObservableCollection<TagLayoutModel>()
                {
                    new TagLayoutModel() {ServerId = "PainterPaint",       LocalName = AppResources.PainterPaintLocalName},
                    new TagLayoutModel() {ServerId = "PainterWallpaper",   LocalName = AppResources.PainterWallpaperLocalName}
                }
            },
            new CraftLayoutModel()
            {
                ServerId = "Plumber",
                Code = 4,
                LocalName = AppResources.PlumberLocalName,
                LocalDescription = AppResources.PlumberLocalDescription,
                AddTaskIcon = "wrensh_green",
                FilterIcon = "screwdriver_fade",
                LocalFullDescription = AppResources.PlumberLocalFullDescription,
                LocalJobDescriptionPlaceholder = AppResources.PlumberLocalJobDescriptionPlaceholder,
                Tags = new ObservableCollection<TagLayoutModel>()
                {
                    new TagLayoutModel() {ServerId = "PlumberRadiator",             LocalName = AppResources.PlumberRadiatorLocalName},
                    new TagLayoutModel() {ServerId = "PlumberToilet",               LocalName = AppResources.PlumberToiletLocalName},
                    new TagLayoutModel() {ServerId = "PlumberPiping",               LocalName = AppResources.PlumberPipingLocalName},
                    new TagLayoutModel() {ServerId = "PlumberFaucet",               LocalName = AppResources.PlumberFaucetLocalName},
                    new TagLayoutModel() {ServerId = "PlumberShower",               LocalName = AppResources.PlumberShowerLocalName},
                    new TagLayoutModel() {ServerId = "PlumberShowerCabin",          LocalName = AppResources.PlumberShowerCabinLocalName},
                    new TagLayoutModel() {ServerId = "PlumberSink",                 LocalName = AppResources.PlumberSinkLocalName},
                    new TagLayoutModel() {ServerId = "PlumberBathtub",              LocalName = AppResources.PlumberBathtubLocalName},
                    new TagLayoutModel() {ServerId = "PlumberSolarHeating",         LocalName = AppResources.PlumberSolarHeatingLocalName},
                    new TagLayoutModel() {ServerId = "PlumberHeatPumpsBoilers",     LocalName = AppResources.PlumberHeatPumpsBoilersLocalName}
                }
            },
            new CraftLayoutModel()
            {
                ServerId = "FloorLayer",
                Code = 5,
                LocalName = AppResources.FloorLayerLocalName,
                LocalDescription = AppResources.FloorLayerLocalDescription,
                AddTaskIcon = "saw_green",
                FilterIcon = "screwdriver_fade",
                LocalFullDescription = AppResources.FloorLayerLocalFullDescription, 
                LocalJobDescriptionPlaceholder = AppResources.FloorLayerLocalJobDescriptionPlaceholder,
                Tags = new ObservableCollection<TagLayoutModel>()
                {
                    new TagLayoutModel() {ServerId = "FloorLayerCarpet",            LocalName = AppResources.FloorLayerCarpetLocalName},
                    new TagLayoutModel() {ServerId = "FloorLayerFloor",             LocalName = AppResources.FloorLayerFloorLocalName}
                }
            },
            new CraftLayoutModel()
            {
                ServerId = "Tiler",
                Code = 6,
                LocalName = AppResources.TilerLocalName,
                LocalDescription = AppResources.TilerLocalDescription,
                AddTaskIcon = "shovel_green",
                FilterIcon = "screwdriver_fade",
                LocalFullDescription = AppResources.TilerLocalFullDescription,
                LocalJobDescriptionPlaceholder = AppResources.TilerLocalJobDescriptionPlaceholder,
                Tags = new ObservableCollection<TagLayoutModel>()
                {
                    new TagLayoutModel() {ServerId = "TilerTiles",          LocalName = AppResources.TilerTilesLocalName},
                    new TagLayoutModel() {ServerId = "TilerClinkers",       LocalName = AppResources.TilerClinkersLocalName}
                }
            },
        };

        public const decimal BaseRootDeductible = 0.3m;
        public const decimal VatRate = 0.25m;
        
        public const int MinTaskMedia = 1;
        public const int MaxTaskMediaPhotos = 8;
        public const int MaxTaskMediaVideos = 2;
        
                
        // android notification stuff
        public const string NotificationIdKey = "notification_id";
        public const string DataKey = "data";
        public const string TitleKey = "title";
        public const string BodyKey = "body";
        public const string NotificationViewModelIdKey = "view_model_id";
        public const string NotificationImageUrlKey = "notification_image_url";
        public const string NotificationBadgeKey = "badge";
        public const string NotificationIdsToDelete = "notification_ids_to_delete";
        public const string RepliedMessageKey = "replied-message";
        public static readonly string GeneralChannelId = "general_notification_channel";
        public static readonly int NotificationId = 100;
        
        // session timeout
        public const int SessionTimeoutDuration = 1000 * 60 * 60; // 60 minutes
        public const int BankIdSessionTimeoutDays = 90; // 90 days
        public const string SessionTimeoutMessage = "SessionTimeout";
        
        // user auth events
        public const string UserLoggedInEventMessage = "UserLoggedIn";
        public const string UserLoggedOutEventMessage = "UserLoggedOut";
    }
}