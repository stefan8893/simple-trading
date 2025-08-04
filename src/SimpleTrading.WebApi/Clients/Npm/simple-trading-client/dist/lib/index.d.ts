export interface ISimpleTradingClient {
    /**
     * @return OK
     */
    getAppInfo(signal?: AbortSignal): Promise<SimpleTradingClientResponse<ApiInfo>>;
    /**
     * @return OK
     */
    getUserSettings(signal?: AbortSignal): Promise<SimpleTradingClientResponse<UserSettingsDto>>;
    /**
     * @return No Content
     */
    updateUserSettings(body: UpdateUserSettingsDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return OK
     */
    getUserLocalNow(signal?: AbortSignal): Promise<SimpleTradingClientResponse<Date>>;
    /**
     * @return OK
     */
    getAvailableTimezones(signal?: AbortSignal): Promise<SimpleTradingClientResponse<TimeZoneOption[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getAssets(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<AssetDto[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getCurrencies(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<CurrencyDto[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getProfiles(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ProfileDto[]>>;
    /**
     * @return OK
     */
    getActiveProfile(signal?: AbortSignal): Promise<SimpleTradingClientResponse<ProfileDto>>;
    /**
     * @return OK
     */
    getReference(tradeId: string, referenceId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ReferenceDto>>;
    /**
     * @return No Content
     */
    updateReference(tradeId: string, referenceId: string, body: UpdateReferenceDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return No Content
     */
    deleteReference(tradeId: string, referenceId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return OK
     */
    getReferences(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ReferenceDto[]>>;
    /**
     * @return OK
     */
    addReference(tradeId: string, body: AddReferenceDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<string>>;
    /**
     * @return OK
     */
    deleteReferences(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<number>>;
    /**
     * @param profileId (optional)
     * @param sort (optional)
     * @param filter (optional)
     * @param page (optional)
     * @param pageSize (optional)
     * @return OK
     */
    searchTrades(profileId: string | undefined, sort: string[] | undefined, filter: string[] | undefined, page: number | undefined, pageSize: number | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<PageDtoOfTradeDto>>;
    /**
     * @return OK
     */
    addTrade(body: AddTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<AddTradeResultDto>>;
    /**
     * @return OK
     */
    getTrade(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeDto>>;
    /**
     * @return OK
     */
    updateTrade(tradeId: string, body: UpdateTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<WarningsDto>>;
    /**
     * @return No Content
     */
    deleteTrade(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return OK
     */
    closeTrade(tradeId: string, body: CloseTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeResultDto>>;
    /**
     * @return OK
     */
    restoreCalculatedResult(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeResultDto>>;
}
export declare class SimpleTradingClient implements ISimpleTradingClient {
    private http;
    private baseUrl;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined;
    constructor(baseUrl?: string, http?: {
        fetch(url: RequestInfo, init?: RequestInit): Promise<Response>;
    });
    /**
     * @return OK
     */
    getAppInfo(signal?: AbortSignal): Promise<SimpleTradingClientResponse<ApiInfo>>;
    protected processGetAppInfo(response: Response): Promise<SimpleTradingClientResponse<ApiInfo>>;
    /**
     * @return OK
     */
    getUserSettings(signal?: AbortSignal): Promise<SimpleTradingClientResponse<UserSettingsDto>>;
    protected processGetUserSettings(response: Response): Promise<SimpleTradingClientResponse<UserSettingsDto>>;
    /**
     * @return No Content
     */
    updateUserSettings(body: UpdateUserSettingsDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;
    protected processUpdateUserSettings(response: Response): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return OK
     */
    getUserLocalNow(signal?: AbortSignal): Promise<SimpleTradingClientResponse<Date>>;
    protected processGetUserLocalNow(response: Response): Promise<SimpleTradingClientResponse<Date>>;
    /**
     * @return OK
     */
    getAvailableTimezones(signal?: AbortSignal): Promise<SimpleTradingClientResponse<TimeZoneOption[]>>;
    protected processGetAvailableTimezones(response: Response): Promise<SimpleTradingClientResponse<TimeZoneOption[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getAssets(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<AssetDto[]>>;
    protected processGetAssets(response: Response): Promise<SimpleTradingClientResponse<AssetDto[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getCurrencies(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<CurrencyDto[]>>;
    protected processGetCurrencies(response: Response): Promise<SimpleTradingClientResponse<CurrencyDto[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getProfiles(searchTerm: string | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ProfileDto[]>>;
    protected processGetProfiles(response: Response): Promise<SimpleTradingClientResponse<ProfileDto[]>>;
    /**
     * @return OK
     */
    getActiveProfile(signal?: AbortSignal): Promise<SimpleTradingClientResponse<ProfileDto>>;
    protected processGetActiveProfile(response: Response): Promise<SimpleTradingClientResponse<ProfileDto>>;
    /**
     * @return OK
     */
    getReference(tradeId: string, referenceId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ReferenceDto>>;
    protected processGetReference(response: Response): Promise<SimpleTradingClientResponse<ReferenceDto>>;
    /**
     * @return No Content
     */
    updateReference(tradeId: string, referenceId: string, body: UpdateReferenceDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;
    protected processUpdateReference(response: Response): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return No Content
     */
    deleteReference(tradeId: string, referenceId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;
    protected processDeleteReference(response: Response): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return OK
     */
    getReferences(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<ReferenceDto[]>>;
    protected processGetReferences(response: Response): Promise<SimpleTradingClientResponse<ReferenceDto[]>>;
    /**
     * @return OK
     */
    addReference(tradeId: string, body: AddReferenceDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<string>>;
    protected processAddReference(response: Response): Promise<SimpleTradingClientResponse<string>>;
    /**
     * @return OK
     */
    deleteReferences(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<number>>;
    protected processDeleteReferences(response: Response): Promise<SimpleTradingClientResponse<number>>;
    /**
     * @param profileId (optional)
     * @param sort (optional)
     * @param filter (optional)
     * @param page (optional)
     * @param pageSize (optional)
     * @return OK
     */
    searchTrades(profileId: string | undefined, sort: string[] | undefined, filter: string[] | undefined, page: number | undefined, pageSize: number | undefined, signal?: AbortSignal): Promise<SimpleTradingClientResponse<PageDtoOfTradeDto>>;
    protected processSearchTrades(response: Response): Promise<SimpleTradingClientResponse<PageDtoOfTradeDto>>;
    /**
     * @return OK
     */
    addTrade(body: AddTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<AddTradeResultDto>>;
    protected processAddTrade(response: Response): Promise<SimpleTradingClientResponse<AddTradeResultDto>>;
    /**
     * @return OK
     */
    getTrade(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeDto>>;
    protected processGetTrade(response: Response): Promise<SimpleTradingClientResponse<TradeDto>>;
    /**
     * @return OK
     */
    updateTrade(tradeId: string, body: UpdateTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<WarningsDto>>;
    protected processUpdateTrade(response: Response): Promise<SimpleTradingClientResponse<WarningsDto>>;
    /**
     * @return No Content
     */
    deleteTrade(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<void>>;
    protected processDeleteTrade(response: Response): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return OK
     */
    closeTrade(tradeId: string, body: CloseTradeDto, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeResultDto>>;
    protected processCloseTrade(response: Response): Promise<SimpleTradingClientResponse<TradeResultDto>>;
    /**
     * @return OK
     */
    restoreCalculatedResult(tradeId: string, signal?: AbortSignal): Promise<SimpleTradingClientResponse<TradeResultDto>>;
    protected processRestoreCalculatedResult(response: Response): Promise<SimpleTradingClientResponse<TradeResultDto>>;
}
export declare class AddReferenceDto implements IAddReferenceDto {
    type?: NullableOfReferenceTypeDto | undefined;
    link?: string | undefined;
    notes?: string | undefined;
    [key: string]: any;
    constructor(data?: IAddReferenceDto);
    init(_data?: any): void;
    static fromJS(data: any): AddReferenceDto;
    toJSON(data?: any): any;
}
export interface IAddReferenceDto {
    type?: NullableOfReferenceTypeDto | undefined;
    link?: string | undefined;
    notes?: string | undefined;
    [key: string]: any;
}
export declare class AddTradeDto implements IAddTradeDto {
    dryRun?: boolean | undefined;
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;
    profitLoss?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    notes?: string | undefined;
    references?: AddReferenceDto[] | undefined;
    [key: string]: any;
    constructor(data?: IAddTradeDto);
    init(_data?: any): void;
    static fromJS(data: any): AddTradeDto;
    toJSON(data?: any): any;
}
export interface IAddTradeDto {
    dryRun?: boolean | undefined;
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;
    profitLoss?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    notes?: string | undefined;
    references?: AddReferenceDto[] | undefined;
    [key: string]: any;
}
export declare class AddTradeResultDto implements IAddTradeResultDto {
    tradeId: string;
    warnings: string[];
    [key: string]: any;
    constructor(data?: IAddTradeResultDto);
    init(_data?: any): void;
    static fromJS(data: any): AddTradeResultDto;
    toJSON(data?: any): any;
}
export interface IAddTradeResultDto {
    tradeId: string;
    warnings: string[];
    [key: string]: any;
}
export declare class ApiInfo implements IApiInfo {
    name: string;
    version: string;
    environment: string;
    [key: string]: any;
    constructor(data?: IApiInfo);
    init(_data?: any): void;
    static fromJS(data: any): ApiInfo;
    toJSON(data?: any): any;
}
export interface IApiInfo {
    name: string;
    version: string;
    environment: string;
    [key: string]: any;
}
export declare class AssetDto implements IAssetDto {
    id: string;
    symbol: string;
    name: string;
    [key: string]: any;
    constructor(data?: IAssetDto);
    init(_data?: any): void;
    static fromJS(data: any): AssetDto;
    toJSON(data?: any): any;
}
export interface IAssetDto {
    id: string;
    symbol: string;
    name: string;
    [key: string]: any;
}
export declare class CloseTradeDto implements ICloseTradeDto {
    profitLoss?: number | undefined;
    exitPrice?: number | undefined;
    closed?: Date | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;
    [key: string]: any;
    constructor(data?: ICloseTradeDto);
    init(_data?: any): void;
    static fromJS(data: any): CloseTradeDto;
    toJSON(data?: any): any;
}
export interface ICloseTradeDto {
    profitLoss?: number | undefined;
    exitPrice?: number | undefined;
    closed?: Date | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;
    [key: string]: any;
}
export declare class CurrencyDto implements ICurrencyDto {
    id: string;
    isoCode: string;
    name: string;
    [key: string]: any;
    constructor(data?: ICurrencyDto);
    init(_data?: any): void;
    static fromJS(data: any): CurrencyDto;
    toJSON(data?: any): any;
}
export interface ICurrencyDto {
    id: string;
    isoCode: string;
    name: string;
    [key: string]: any;
}
export declare class ErrorResponse implements IErrorResponse {
    messages: string[];
    [key: string]: any;
    constructor(data?: IErrorResponse);
    init(_data?: any): void;
    static fromJS(data: any): ErrorResponse;
    toJSON(data?: any): any;
}
export interface IErrorResponse {
    messages: string[];
    [key: string]: any;
}
export declare class FieldError implements IFieldError {
    identifier: string;
    messages: string[];
    [key: string]: any;
    constructor(data?: IFieldError);
    init(_data?: any): void;
    static fromJS(data: any): FieldError;
    toJSON(data?: any): any;
}
export interface IFieldError {
    identifier: string;
    messages: string[];
    [key: string]: any;
}
export declare class FieldErrorResponse implements IFieldErrorResponse {
    errors: FieldError[];
    [key: string]: any;
    constructor(data?: IFieldErrorResponse);
    init(_data?: any): void;
    static fromJS(data: any): FieldErrorResponse;
    toJSON(data?: any): any;
}
export interface IFieldErrorResponse {
    errors: FieldError[];
    [key: string]: any;
}
export type NullableOfReferenceTypeDto = "TradingView" | "Other";
export type NullableOfResultDto = "Win" | "Mediocre" | "BreakEven" | "Loss";
export declare class PageDtoOfTradeDto implements IPageDtoOfTradeDto {
    data: TradeDto[];
    count: number;
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    [key: string]: any;
    constructor(data?: IPageDtoOfTradeDto);
    init(_data?: any): void;
    static fromJS(data: any): PageDtoOfTradeDto;
    toJSON(data?: any): any;
}
export interface IPageDtoOfTradeDto {
    data: TradeDto[];
    count: number;
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    [key: string]: any;
}
export declare class ProfileDto implements IProfileDto {
    id: string;
    name: string;
    description?: string | undefined;
    isActive: boolean;
    [key: string]: any;
    constructor(data?: IProfileDto);
    init(_data?: any): void;
    static fromJS(data: any): ProfileDto;
    toJSON(data?: any): any;
}
export interface IProfileDto {
    id: string;
    name: string;
    description?: string | undefined;
    isActive: boolean;
    [key: string]: any;
}
export declare class ReferenceDto implements IReferenceDto {
    id: string;
    type: ReferenceTypeDto;
    link: string;
    notes?: string | undefined;
    [key: string]: any;
    constructor(data?: IReferenceDto);
    init(_data?: any): void;
    static fromJS(data: any): ReferenceDto;
    toJSON(data?: any): any;
}
export interface IReferenceDto {
    id: string;
    type: ReferenceTypeDto;
    link: string;
    notes?: string | undefined;
    [key: string]: any;
}
export type ReferenceTypeDto = "TradingView" | "Other";
export declare class TimeZoneOption implements ITimeZoneOption {
    windowsId: string;
    timeZone: string;
    offset: string;
    [key: string]: any;
    constructor(data?: ITimeZoneOption);
    init(_data?: any): void;
    static fromJS(data: any): TimeZoneOption;
    toJSON(data?: any): any;
}
export interface ITimeZoneOption {
    windowsId: string;
    timeZone: string;
    offset: string;
    [key: string]: any;
}
export declare class TradeDto implements ITradeDto {
    id?: string;
    assetId?: string;
    asset: string;
    profileId?: string;
    profile: string;
    size?: number;
    opened?: Date;
    closed?: Date | undefined;
    profitLoss?: number | undefined;
    result?: NullableOfResultDto | undefined;
    performance?: number | undefined;
    isClosed?: boolean;
    currencyId?: string;
    currency: string;
    entry?: number;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    riskRewardRatio?: number | undefined;
    references: ReferenceDto[];
    notes?: string | undefined;
    warnings: string[];
    [key: string]: any;
    constructor(data?: ITradeDto);
    init(_data?: any): void;
    static fromJS(data: any): TradeDto;
    toJSON(data?: any): any;
}
export interface ITradeDto {
    id?: string;
    assetId?: string;
    asset: string;
    profileId?: string;
    profile: string;
    size?: number;
    opened?: Date;
    closed?: Date | undefined;
    profitLoss?: number | undefined;
    result?: NullableOfResultDto | undefined;
    performance?: number | undefined;
    isClosed?: boolean;
    currencyId?: string;
    currency: string;
    entry?: number;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    riskRewardRatio?: number | undefined;
    references: ReferenceDto[];
    notes?: string | undefined;
    warnings: string[];
    [key: string]: any;
}
export declare class TradeResultDto implements ITradeResultDto {
    tradeId: string;
    result: NullableOfResultDto | undefined;
    performance: number | undefined;
    warnings: string[];
    [key: string]: any;
    constructor(data?: ITradeResultDto);
    init(_data?: any): void;
    static fromJS(data: any): TradeResultDto;
    toJSON(data?: any): any;
}
export interface ITradeResultDto {
    tradeId: string;
    result: NullableOfResultDto | undefined;
    performance: number | undefined;
    warnings: string[];
    [key: string]: any;
}
export declare class UpdateReferenceDto implements IUpdateReferenceDto {
    type?: NullableOfReferenceTypeDto | undefined;
    link?: string | undefined;
    notes?: UpdateStringValue | undefined;
    [key: string]: any;
    constructor(data?: IUpdateReferenceDto);
    init(_data?: any): void;
    static fromJS(data: any): UpdateReferenceDto;
    toJSON(data?: any): any;
}
export interface IUpdateReferenceDto {
    type?: NullableOfReferenceTypeDto | undefined;
    link?: string | undefined;
    notes?: UpdateStringValue | undefined;
    [key: string]: any;
}
export declare class UpdateResultValue implements IUpdateResultValue {
    value?: NullableOfResultDto | undefined;
    [key: string]: any;
    constructor(data?: IUpdateResultValue);
    init(_data?: any): void;
    static fromJS(data: any): UpdateResultValue;
    toJSON(data?: any): any;
}
export interface IUpdateResultValue {
    value?: NullableOfResultDto | undefined;
    [key: string]: any;
}
export declare class UpdateStringValue implements IUpdateStringValue {
    value?: string | undefined;
    [key: string]: any;
    constructor(data?: IUpdateStringValue);
    init(_data?: any): void;
    static fromJS(data: any): UpdateStringValue;
    toJSON(data?: any): any;
}
export interface IUpdateStringValue {
    value?: string | undefined;
    [key: string]: any;
}
export declare class UpdateTradeDto implements IUpdateTradeDto {
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;
    profitLoss?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: UpdateValueOfNullableOfdecimal | undefined;
    takeProfit?: UpdateValueOfNullableOfdecimal | undefined;
    exitPrice?: UpdateValueOfNullableOfdecimal | undefined;
    notes?: UpdateStringValue | undefined;
    [key: string]: any;
    constructor(data?: IUpdateTradeDto);
    init(_data?: any): void;
    static fromJS(data: any): UpdateTradeDto;
    toJSON(data?: any): any;
}
export interface IUpdateTradeDto {
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    manuallyEnteredResult?: UpdateResultValue | undefined;
    profitLoss?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: UpdateValueOfNullableOfdecimal | undefined;
    takeProfit?: UpdateValueOfNullableOfdecimal | undefined;
    exitPrice?: UpdateValueOfNullableOfdecimal | undefined;
    notes?: UpdateStringValue | undefined;
    [key: string]: any;
}
export declare class UpdateUserSettingsDto implements IUpdateUserSettingsDto {
    culture?: string | undefined;
    isoLanguageCode?: UpdateStringValue | undefined;
    timeZone?: string | undefined;
    [key: string]: any;
    constructor(data?: IUpdateUserSettingsDto);
    init(_data?: any): void;
    static fromJS(data: any): UpdateUserSettingsDto;
    toJSON(data?: any): any;
}
export interface IUpdateUserSettingsDto {
    culture?: string | undefined;
    isoLanguageCode?: UpdateStringValue | undefined;
    timeZone?: string | undefined;
    [key: string]: any;
}
export declare class UpdateValueOfNullableOfdecimal implements IUpdateValueOfNullableOfdecimal {
    value?: number | undefined;
    [key: string]: any;
    constructor(data?: IUpdateValueOfNullableOfdecimal);
    init(_data?: any): void;
    static fromJS(data: any): UpdateValueOfNullableOfdecimal;
    toJSON(data?: any): any;
}
export interface IUpdateValueOfNullableOfdecimal {
    value?: number | undefined;
    [key: string]: any;
}
export declare class UserSettingsDto implements IUserSettingsDto {
    culture: string;
    language: string | undefined;
    timeZone: string;
    lastModified: Date;
    [key: string]: any;
    constructor(data?: IUserSettingsDto);
    init(_data?: any): void;
    static fromJS(data: any): UserSettingsDto;
    toJSON(data?: any): any;
}
export interface IUserSettingsDto {
    culture: string;
    language: string | undefined;
    timeZone: string;
    lastModified: Date;
    [key: string]: any;
}
export declare class WarningsDto implements IWarningsDto {
    warnings: string[];
    [key: string]: any;
    constructor(data?: IWarningsDto);
    init(_data?: any): void;
    static fromJS(data: any): WarningsDto;
    toJSON(data?: any): any;
}
export interface IWarningsDto {
    warnings: string[];
    [key: string]: any;
}
export declare class SimpleTradingClientResponse<TResult> {
    status: number;
    headers: {
        [key: string]: any;
    };
    result: TResult;
    constructor(status: number, headers: {
        [key: string]: any;
    }, result: TResult);
}
export declare class SimpleTradingClientException extends Error {
    message: string;
    status: number;
    response: string;
    headers: {
        [key: string]: any;
    };
    result: any;
    constructor(message: string, status: number, response: string, headers: {
        [key: string]: any;
    }, result: any);
    protected isSimpleTradingClientException: boolean;
    static isSimpleTradingClientException(obj: any): obj is SimpleTradingClientException;
}
