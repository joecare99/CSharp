﻿namespace GenInterfaces.Data;

public enum EGedComTags
{
    Ged_None = 0,
    Ged_ABBR = 60, // Abbreviation
    Ged_ADDR = 22, // Address
    Ged_ADR1 = 23, // Address1
    Ged_ADR2 = 24, // Address2
    Ged_ADR3 = 25, // Address3
    Ged_ADR4 = 26, // Address4 ?
    Ged_ADOP = 61, // Adopted
    Ged_AFN = 62, // Ancestral File Number
    Ged_AGE = 63, // Age
    Ged_AGNC = 64, // Agency
    Ged_ALIA = 65, // Alias
    Ged_ANCE = 66, // Ancestor
    Ged_ANCI = 67, // Ancestor Interest
    Ged_ANUL = 68, // Annulment
    Ged_ASSO = 69, // Association
    Ged_AUTH = 44, // Author
    Ged_BAPL = 58, // Baptism-LDS
    Ged_BAPM = 57, // Baptism
    Ged_BARM = 59, // Bar Mitzvah
    Ged_BASM = 70, // Bas Mitzvah
    Ged_BIRT = 45, // Birth
    Ged_BLES = 71, // Blessing
    Ged_BURI = 47, // Burial
    Ged_CALN = 17, // Call Number
    Ged_CAST = 72, // Caste
    Ged_CAUS = 73, // Cause
    Ged_CENS = 74, // Census
    Ged_CHAN = 75, // Change
    Ged_CHAR = 37, // Character Set
    Ged_CHIL = 76, // Child
    Ged_CHR = 77, // Christening
    Ged_CHRA = 78, // Christening-Adult
    Ged_CITY = 27, // City
    Ged_CONC = 14, // Concatenation
    Ged_CONF = 79, // Confirmation
    Ged_CONL = 80, // Confirmation-LDS
    Ged_CONT = 15, // Continuation
    Ged_COPR = 7, // Copyright
    Ged_CORP = 81, // Corporation
    Ged_CREM = 82, // Cremation
    Ged_CTRY = 28, // Country
    Ged_DATA = 19, // Data
    Ged_DATE = 38, // Date
    Ged_DEAT = 46, // Death
    Ged_DESC = 32, // Description
    Ged_DESI = 83, // Descendant Interest
    Ged_DEST = 84, // Destination
    Ged_DIV = 85, // Divorce
    Ged_DIVF = 86, // Divorce Filed
    Ged_DSCR = 87, // (physical) Description
    Ged_EDUC = 88, // Education
    Ged_EMAIL = 33, // Email
    Ged_EMIG = 89, // Emigration
    Ged_ENDL = 90, // Endowment-LDS
    Ged_ENGA = 91, // Engagement
    Ged_EVEN = 92, // Event
    Ged_FACT = 93, // Fact
    Ged_FAM = 3, // Family
    Ged_FAMC = 94, // Family Child
    Ged_FAMF = 95, // Family File
    Ged_FAMS = 4, // Family Spouse
    Ged_FAX = 34, // Fax
    Ged_FCOM = 96, // First Communion
    Ged_FILE = 6, // File
    Ged_FONE = 97, // Phonetic variation
    Ged_FORM = 10, // Format
    Ged_GEDC = 8, // GedCom
    Ged_GIVN = 40, // Given Name
    Ged_GRAD = 98, // Graduation
    Ged_HEAD = 1, // Header
    Ged_HUSB = 99, // Husband
    Ged_IDNO = 100, // Identification Number
    Ged_IMMI = 101, // Immigration 
    Ged_LANG = 11, // Language
    Ged_LATI = 102, // Latitude
    Ged_LONG = 103, // Longitude
    Ged_MAP = 20, // Map
    Ged_MARB = 104, // Marriage Bann
    Ged_MARC = 36, // Marriage Contract
    Ged_MARL = 105, // Marriage License
    Ged_MARR = 106, // Marriage
    Ged_MARS = 107, // Marriage Settlement
    Ged_MEDI = 18, // Media
    Ged_NAME = 2, // Name
    Ged_NATI = 108, // Nationality
    Ged_NATU = 109, // Naturalization
    Ged_NCHI = 110, // Number of Children
    Ged_NICK = 42, // Nickname
    Ged_NMR = 111, // Number of Marriages
    Ged_NOTE = 13, // Note
    Ged_NPFX = 112, // Name Prefix
    Ged_OBJE = 21, // Object
    Ged_OCCU = 50, // Occupation
    Ged_ORDI = 113, // Ordinance
    Ged_ORDN = 114, // Ordination
    Ged_PAGE = 115, // Page
    Ged_PEDI = 116, // Pedigree
    Ged_PHON = 52, // Phone
    Ged_PLAC = 12, // Place
    Ged_POST = 30, // Postal Code
    Ged_PROB = 117, // Probate
    Ged_PROP = 118, // Property
    Ged_PUBL = 119, // Publication
    Ged_QUAY = 51, // Quality of Data
    Ged_REFN = 51, // Reference Number
    Ged_RELA = 29, // Relationship
    Ged_RELI = 55, // Religion
    Ged_REPO = 16, // Repository
    Ged_RESI = 49, // Residence
    Ged_RESN = 120, // Restriction
    Ged_RETI = 121, // Retirement
    Ged_RFN = 122, // Rec. File Number (unique)
    Ged_RIN = 123, // Rec. ID Number
    Ged_ROLE = 124, // Role
    Ged_ROMN = 125, // Romanized
    Ged_SEX = 56, // Sex
    Ged_SLGC = 126, // Sealing Child
    Ged_SLGS = 127, // Sealing Spouse
    Ged_SOUR = 43, // Source
    Ged_SPFX = 128, // Surname Prefix
    Ged_SSN = 53, // Social Security Number
    Ged_STAE = 31, // State
    Ged_STAT = 129, // Status
    Ged_SUBM = 39, // Submitter
    Ged_SUBN = 5, // Submission
    Ged_SURN = 41, // Surname
    Ged_TEMP = 130, // Temple
    Ged_TEXT = 131, // Text
    Ged_TIME = 132, // Time
    Ged_TITL = 133, // Title
    Ged_TRLR = 134, // Trailer
    Ged_VERS = 9, // Version
    Ged_WIFE = 135, // Wife
    Ged_WWW = 35, // WWW
    Ged_WILL = 136, // Will
    Ged__UID = 48, // Unique ID
    Ged__RUFNAME = 52, // Call-Name
    Ged__GOV = 53, // Genealogical-Place-Authority
    Ged__LOC = 54, // Location
}