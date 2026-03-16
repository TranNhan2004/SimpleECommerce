1. Read the documents of Angular 21 on the internet to understand the new way to code.
2. ONLY use `signal()` and the related-things of it, NOT USE `@Input()` or `@Output` old.
3. Create file without .component or .services, ... as the old version.
4. When working with `Observable` sources, use `@angular/core/rxjs-interop` features such as `toSignal()` or `toObservable()` to bridge RxJS with Signals. All Angular state and UI-facing data flow must end in Signals style, not manual subscriptions or legacy RxJS-only component patterns.
