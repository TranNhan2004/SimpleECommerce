import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { App } from './app';
import { AuthService } from './core/services/auth.service';
import { TranslocoService } from '@jsverse/transloco';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [
        provideRouter([]),
        {
          provide: TranslocoService,
          useValue: {
            setActiveLang: () => undefined,
            selectTranslation: () => of({}),
            translate: (key: string) => key,
            getActiveLang: () => 'en'
          }
        },
        {
          provide: AuthService,
          useValue: {
            authenticated: () => false,
            loading: () => false,
            displayName: () => 'Guest',
            username: () => null,
            accessToken: () => null,
            realmRoles: () => [],
            resourceRoles: () => [],
            getRoles: () => [],
            login: async () => undefined,
            register: async () => undefined,
            logout: async () => undefined,
            hasRole: () => false
          }
        }
      ],
      imports: [App],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.se-nav')).toBeTruthy();
  });
});
