export interface ISimpleTradingClient {
    /**
     * @return OK
     */
    getAppInfo(): Promise<SimpleTradingClientResponse<ApiInfo>>;
    /**
     * @param profileId (optional)
     * @param sort (optional)
     * @param filter (optional)
     * @param page (optional)
     * @param pageSize (optional)
     * @return OK
     */
    searchTrades(profileId: string | undefined, sort: string[] | undefined, filter: string[] | undefined, page: number | undefined, pageSize: number | undefined): Promise<SimpleTradingClientResponse<TradeDtoPageDto>>;
    /**
     * @param body (optional)
     * @return OK
     */
    addTrade(body: AddTradeDto | undefined): Promise<SimpleTradingClientResponse<AddTradeResultDto>>;
    /**
     * @return OK
     */
    getTrade(tradeId: string): Promise<SimpleTradingClientResponse<TradeDto>>;
    /**
     * @param body (optional)
     * @return OK
     */
    updateTrade(tradeId: string, body: UpdateTradeDto | undefined): Promise<SimpleTradingClientResponse<WarningsDto>>;
    /**
     * @return No Content
     */
    deleteTrade(tradeId: string): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @param body (optional)
     * @return OK
     */
    closeTrade(tradeId: string, body: CloseTradeDto | undefined): Promise<SimpleTradingClientResponse<TradeResultDto>>;
    /**
     * @return OK
     */
    restoreCalculatedResult(tradeId: string): Promise<SimpleTradingClientResponse<TradeResultDto>>;
    /**
     * @return OK
     */
    getReference(tradeId: string, referenceId: string): Promise<SimpleTradingClientResponse<ReferenceDto>>;
    /**
     * @param body (optional)
     * @return No Content
     */
    updateReference(tradeId: string, referenceId: string, body: UpdateReferenceDto | undefined): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return No Content
     */
    deleteReference(tradeId: string, referenceId: string): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return OK
     */
    getReferences(tradeId: string): Promise<SimpleTradingClientResponse<ReferenceDto[]>>;
    /**
     * @param body (optional)
     * @return OK
     */
    addReference(tradeId: string, body: AddReferenceDto | undefined): Promise<SimpleTradingClientResponse<string>>;
    /**
     * @return OK
     */
    deleteReferences(tradeId: string): Promise<SimpleTradingClientResponse<number>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getProfiles(searchTerm: string | undefined): Promise<SimpleTradingClientResponse<ProfileDto[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getAssets(searchTerm: string | undefined): Promise<SimpleTradingClientResponse<AssetDto[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getCurrencies(searchTerm: string | undefined): Promise<SimpleTradingClientResponse<CurrencyDto[]>>;
    /**
     * @return OK
     */
    getUserSettings(): Promise<SimpleTradingClientResponse<UserSettingsDto>>;
    /**
     * @param body (optional)
     * @return No Content
     */
    updateUserSettings(body: UpdateUserSettingsDto | undefined): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return OK
     */
    getUserLocalNow(): Promise<SimpleTradingClientResponse<Date>>;
    /**
     * @return OK
     */
    getAvailableTimezones(): Promise<SimpleTradingClientResponse<TimeZoneOption[]>>;
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
    getAppInfo(): Promise<SimpleTradingClientResponse<ApiInfo>>;
    protected processGetAppInfo(response: Response): Promise<SimpleTradingClientResponse<ApiInfo>>;
    /**
     * @param profileId (optional)
     * @param sort (optional)
     * @param filter (optional)
     * @param page (optional)
     * @param pageSize (optional)
     * @return OK
     */
    searchTrades(profileId: string | undefined, sort: string[] | undefined, filter: string[] | undefined, page: number | undefined, pageSize: number | undefined): Promise<SimpleTradingClientResponse<TradeDtoPageDto>>;
    protected processSearchTrades(response: Response): Promise<SimpleTradingClientResponse<TradeDtoPageDto>>;
    /**
     * @param body (optional)
     * @return OK
     */
    addTrade(body: AddTradeDto | undefined): Promise<SimpleTradingClientResponse<AddTradeResultDto>>;
    protected processAddTrade(response: Response): Promise<SimpleTradingClientResponse<AddTradeResultDto>>;
    /**
     * @return OK
     */
    getTrade(tradeId: string): Promise<SimpleTradingClientResponse<TradeDto>>;
    protected processGetTrade(response: Response): Promise<SimpleTradingClientResponse<TradeDto>>;
    /**
     * @param body (optional)
     * @return OK
     */
    updateTrade(tradeId: string, body: UpdateTradeDto | undefined): Promise<SimpleTradingClientResponse<WarningsDto>>;
    protected processUpdateTrade(response: Response): Promise<SimpleTradingClientResponse<WarningsDto>>;
    /**
     * @return No Content
     */
    deleteTrade(tradeId: string): Promise<SimpleTradingClientResponse<void>>;
    protected processDeleteTrade(response: Response): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @param body (optional)
     * @return OK
     */
    closeTrade(tradeId: string, body: CloseTradeDto | undefined): Promise<SimpleTradingClientResponse<TradeResultDto>>;
    protected processCloseTrade(response: Response): Promise<SimpleTradingClientResponse<TradeResultDto>>;
    /**
     * @return OK
     */
    restoreCalculatedResult(tradeId: string): Promise<SimpleTradingClientResponse<TradeResultDto>>;
    protected processRestoreCalculatedResult(response: Response): Promise<SimpleTradingClientResponse<TradeResultDto>>;
    /**
     * @return OK
     */
    getReference(tradeId: string, referenceId: string): Promise<SimpleTradingClientResponse<ReferenceDto>>;
    protected processGetReference(response: Response): Promise<SimpleTradingClientResponse<ReferenceDto>>;
    /**
     * @param body (optional)
     * @return No Content
     */
    updateReference(tradeId: string, referenceId: string, body: UpdateReferenceDto | undefined): Promise<SimpleTradingClientResponse<void>>;
    protected processUpdateReference(response: Response): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return No Content
     */
    deleteReference(tradeId: string, referenceId: string): Promise<SimpleTradingClientResponse<void>>;
    protected processDeleteReference(response: Response): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return OK
     */
    getReferences(tradeId: string): Promise<SimpleTradingClientResponse<ReferenceDto[]>>;
    protected processGetReferences(response: Response): Promise<SimpleTradingClientResponse<ReferenceDto[]>>;
    /**
     * @param body (optional)
     * @return OK
     */
    addReference(tradeId: string, body: AddReferenceDto | undefined): Promise<SimpleTradingClientResponse<string>>;
    protected processAddReference(response: Response): Promise<SimpleTradingClientResponse<string>>;
    /**
     * @return OK
     */
    deleteReferences(tradeId: string): Promise<SimpleTradingClientResponse<number>>;
    protected processDeleteReferences(response: Response): Promise<SimpleTradingClientResponse<number>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getProfiles(searchTerm: string | undefined): Promise<SimpleTradingClientResponse<ProfileDto[]>>;
    protected processGetProfiles(response: Response): Promise<SimpleTradingClientResponse<ProfileDto[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getAssets(searchTerm: string | undefined): Promise<SimpleTradingClientResponse<AssetDto[]>>;
    protected processGetAssets(response: Response): Promise<SimpleTradingClientResponse<AssetDto[]>>;
    /**
     * @param searchTerm (optional)
     * @return OK
     */
    getCurrencies(searchTerm: string | undefined): Promise<SimpleTradingClientResponse<CurrencyDto[]>>;
    protected processGetCurrencies(response: Response): Promise<SimpleTradingClientResponse<CurrencyDto[]>>;
    /**
     * @return OK
     */
    getUserSettings(): Promise<SimpleTradingClientResponse<UserSettingsDto>>;
    protected processGetUserSettings(response: Response): Promise<SimpleTradingClientResponse<UserSettingsDto>>;
    /**
     * @param body (optional)
     * @return No Content
     */
    updateUserSettings(body: UpdateUserSettingsDto | undefined): Promise<SimpleTradingClientResponse<void>>;
    protected processUpdateUserSettings(response: Response): Promise<SimpleTradingClientResponse<void>>;
    /**
     * @return OK
     */
    getUserLocalNow(): Promise<SimpleTradingClientResponse<Date>>;
    protected processGetUserLocalNow(response: Response): Promise<SimpleTradingClientResponse<Date>>;
    /**
     * @return OK
     */
    getAvailableTimezones(): Promise<SimpleTradingClientResponse<TimeZoneOption[]>>;
    protected processGetAvailableTimezones(response: Response): Promise<SimpleTradingClientResponse<TimeZoneOption[]>>;
}
export declare class AddReferenceDto implements IAddReferenceDto {
    type?: ReferenceTypeDto;
    link?: string | undefined;
    notes?: string | undefined;
    constructor(data?: IAddReferenceDto);
    init(_data?: any): void;
    static fromJS(data: any): AddReferenceDto;
    toJSON(data?: any): any;
}
export interface IAddReferenceDto {
    type?: ReferenceTypeDto;
    link?: string | undefined;
    notes?: string | undefined;
}
export declare class AddTradeDto implements IAddTradeDto {
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    manuallyEnteredResult?: ResultDtoNullableUpdateValue;
    balance?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    notes?: string | undefined;
    references?: AddReferenceDto[] | undefined;
    constructor(data?: IAddTradeDto);
    init(_data?: any): void;
    static fromJS(data: any): AddTradeDto;
    toJSON(data?: any): any;
}
export interface IAddTradeDto {
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    manuallyEnteredResult?: ResultDtoNullableUpdateValue;
    balance?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: number | undefined;
    takeProfit?: number | undefined;
    exitPrice?: number | undefined;
    notes?: string | undefined;
    references?: AddReferenceDto[] | undefined;
}
export declare class AddTradeResultDto implements IAddTradeResultDto {
    tradeId: string;
    warnings: string[];
    constructor(data?: IAddTradeResultDto);
    init(_data?: any): void;
    static fromJS(data: any): AddTradeResultDto;
    toJSON(data?: any): any;
}
export interface IAddTradeResultDto {
    tradeId: string;
    warnings: string[];
}
export declare class ApiInfo implements IApiInfo {
    name?: string;
    version?: string;
    environment?: string;
    constructor(data?: IApiInfo);
    init(_data?: any): void;
    static fromJS(data: any): ApiInfo;
    toJSON(data?: any): any;
}
export interface IApiInfo {
    name?: string;
    version?: string;
    environment?: string;
}
export declare class AssetDto implements IAssetDto {
    id: string;
    symbol: string;
    name: string;
    constructor(data?: IAssetDto);
    init(_data?: any): void;
    static fromJS(data: any): AssetDto;
    toJSON(data?: any): any;
}
export interface IAssetDto {
    id: string;
    symbol: string;
    name: string;
}
export declare class CloseTradeDto implements ICloseTradeDto {
    balance?: number | undefined;
    exitPrice?: number | undefined;
    closed?: Date | undefined;
    manuallyEnteredResult?: ResultDtoNullableUpdateValue;
    constructor(data?: ICloseTradeDto);
    init(_data?: any): void;
    static fromJS(data: any): CloseTradeDto;
    toJSON(data?: any): any;
}
export interface ICloseTradeDto {
    balance?: number | undefined;
    exitPrice?: number | undefined;
    closed?: Date | undefined;
    manuallyEnteredResult?: ResultDtoNullableUpdateValue;
}
export declare class CurrencyDto implements ICurrencyDto {
    id: string;
    isoCode: string;
    name: string;
    constructor(data?: ICurrencyDto);
    init(_data?: any): void;
    static fromJS(data: any): CurrencyDto;
    toJSON(data?: any): any;
}
export interface ICurrencyDto {
    id: string;
    isoCode: string;
    name: string;
}
export declare class DecimalNullableUpdateValue implements IDecimalNullableUpdateValue {
    value?: number | undefined;
    constructor(data?: IDecimalNullableUpdateValue);
    init(_data?: any): void;
    static fromJS(data: any): DecimalNullableUpdateValue;
    toJSON(data?: any): any;
}
export interface IDecimalNullableUpdateValue {
    value?: number | undefined;
}
export declare class ErrorResponse implements IErrorResponse {
    messages: string[];
    constructor(data?: IErrorResponse);
    init(_data?: any): void;
    static fromJS(data: any): ErrorResponse;
    toJSON(data?: any): any;
}
export interface IErrorResponse {
    messages: string[];
}
export declare class FieldError implements IFieldError {
    identifier: string;
    messages: string[];
    constructor(data?: IFieldError);
    init(_data?: any): void;
    static fromJS(data: any): FieldError;
    toJSON(data?: any): any;
}
export interface IFieldError {
    identifier: string;
    messages: string[];
}
export declare class FieldErrorResponse implements IFieldErrorResponse {
    errors: FieldError[];
    constructor(data?: IFieldErrorResponse);
    init(_data?: any): void;
    static fromJS(data: any): FieldErrorResponse;
    toJSON(data?: any): any;
}
export interface IFieldErrorResponse {
    errors: FieldError[];
}
export declare class ProfileDto implements IProfileDto {
    id: string;
    name: string;
    description?: string | undefined;
    isActive: boolean;
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
}
export declare class ReferenceDto implements IReferenceDto {
    id: string;
    type: ReferenceTypeDto;
    link: string;
    notes?: string | undefined;
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
}
export declare enum ReferenceTypeDto {
    TradingView = "TradingView",
    Other = "Other"
}
export declare enum ResultDto {
    Win = "Win",
    Mediocre = "Mediocre",
    BreakEven = "BreakEven",
    Loss = "Loss"
}
export declare class ResultDtoNullableUpdateValue implements IResultDtoNullableUpdateValue {
    value?: ResultDto;
    constructor(data?: IResultDtoNullableUpdateValue);
    init(_data?: any): void;
    static fromJS(data: any): ResultDtoNullableUpdateValue;
    toJSON(data?: any): any;
}
export interface IResultDtoNullableUpdateValue {
    value?: ResultDto;
}
export declare class StringUpdateValue implements IStringUpdateValue {
    value?: string | undefined;
    constructor(data?: IStringUpdateValue);
    init(_data?: any): void;
    static fromJS(data: any): StringUpdateValue;
    toJSON(data?: any): any;
}
export interface IStringUpdateValue {
    value?: string | undefined;
}
export declare class TimeZoneOption implements ITimeZoneOption {
    windowsId?: string;
    timeZone?: string;
    offset?: string;
    constructor(data?: ITimeZoneOption);
    init(_data?: any): void;
    static fromJS(data: any): TimeZoneOption;
    toJSON(data?: any): any;
}
export interface ITimeZoneOption {
    windowsId?: string;
    timeZone?: string;
    offset?: string;
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
    balance?: number | undefined;
    result?: ResultDto;
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
    balance?: number | undefined;
    result?: ResultDto;
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
}
export declare class TradeDtoPageDto implements ITradeDtoPageDto {
    data?: TradeDto[];
    count?: number;
    totalCount?: number;
    totalPages?: number;
    page?: number;
    pageSize?: number;
    constructor(data?: ITradeDtoPageDto);
    init(_data?: any): void;
    static fromJS(data: any): TradeDtoPageDto;
    toJSON(data?: any): any;
}
export interface ITradeDtoPageDto {
    data?: TradeDto[];
    count?: number;
    totalCount?: number;
    totalPages?: number;
    page?: number;
    pageSize?: number;
}
export declare class TradeResultDto implements ITradeResultDto {
    tradeId?: string;
    result?: ResultDto;
    performance?: number | undefined;
    warnings?: string[];
    constructor(data?: ITradeResultDto);
    init(_data?: any): void;
    static fromJS(data: any): TradeResultDto;
    toJSON(data?: any): any;
}
export interface ITradeResultDto {
    tradeId?: string;
    result?: ResultDto;
    performance?: number | undefined;
    warnings?: string[];
}
export declare class UpdateReferenceDto implements IUpdateReferenceDto {
    type?: ReferenceTypeDto;
    link?: string | undefined;
    notes?: StringUpdateValue;
    constructor(data?: IUpdateReferenceDto);
    init(_data?: any): void;
    static fromJS(data: any): UpdateReferenceDto;
    toJSON(data?: any): any;
}
export interface IUpdateReferenceDto {
    type?: ReferenceTypeDto;
    link?: string | undefined;
    notes?: StringUpdateValue;
}
export declare class UpdateTradeDto implements IUpdateTradeDto {
    assetId?: string | undefined;
    profileId?: string | undefined;
    opened?: Date | undefined;
    closed?: Date | undefined;
    size?: number | undefined;
    manuallyEnteredResult?: ResultDtoNullableUpdateValue;
    balance?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: DecimalNullableUpdateValue;
    takeProfit?: DecimalNullableUpdateValue;
    exitPrice?: DecimalNullableUpdateValue;
    notes?: StringUpdateValue;
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
    manuallyEnteredResult?: ResultDtoNullableUpdateValue;
    balance?: number | undefined;
    currencyId?: string | undefined;
    entryPrice?: number | undefined;
    stopLoss?: DecimalNullableUpdateValue;
    takeProfit?: DecimalNullableUpdateValue;
    exitPrice?: DecimalNullableUpdateValue;
    notes?: StringUpdateValue;
}
export declare class UpdateUserSettingsDto implements IUpdateUserSettingsDto {
    culture?: string | undefined;
    isoLanguageCode?: StringUpdateValue;
    timeZone?: string | undefined;
    constructor(data?: IUpdateUserSettingsDto);
    init(_data?: any): void;
    static fromJS(data: any): UpdateUserSettingsDto;
    toJSON(data?: any): any;
}
export interface IUpdateUserSettingsDto {
    culture?: string | undefined;
    isoLanguageCode?: StringUpdateValue;
    timeZone?: string | undefined;
}
export declare class UserSettingsDto implements IUserSettingsDto {
    culture: string;
    language: string | undefined;
    timeZone: string;
    lastModified: Date;
    activeProfileId: string;
    activeProfileName: string;
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
    activeProfileId: string;
    activeProfileName: string;
}
export declare class WarningsDto implements IWarningsDto {
    warnings?: string[];
    constructor(data?: IWarningsDto);
    init(_data?: any): void;
    static fromJS(data: any): WarningsDto;
    toJSON(data?: any): any;
}
export interface IWarningsDto {
    warnings?: string[];
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
