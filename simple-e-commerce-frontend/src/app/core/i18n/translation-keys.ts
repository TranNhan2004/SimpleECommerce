import en from '../../../../public/i18n/en.json';

type DotPrefix<TPrefix extends string, TValue extends string> = `${TPrefix}.${TValue}`;

type NestedLeafKey<TValue> = TValue extends string
  ? never
  : {
      [TKey in Extract<keyof TValue, string>]: TValue[TKey] extends string
        ? TKey
        : DotPrefix<TKey, NestedLeafKey<TValue[TKey]>>;
    }[Extract<keyof TValue, string>];


type TranslationSchema = typeof en;
// const vietnameseTranslations: TranslationSchema = vi;
// void vietnameseTranslations;
export type TranslationKey = NestedLeafKey<TranslationSchema>;
export type NavigationTranslationKey = Extract<TranslationKey, `nav.${string}`>;
export type MetaTranslationKey = Extract<TranslationKey, `meta.${string}`>;
export type LanguageTranslationKey = Extract<TranslationKey, `language.${string}`>;
