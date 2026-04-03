import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslocoService } from '@jsverse/transloco';
import { of } from 'rxjs';

import { Translation } from './translation';

@Component({
  selector: 'app-test-translation',
  template: ''
})
class TestTranslationComponent extends Translation {}

describe('Translation', () => {
  let component: TestTranslationComponent;
  let fixture: ComponentFixture<TestTranslationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestTranslationComponent],
      providers: [
        {
          provide: TranslocoService,
          useValue: {
            setActiveLang: () => undefined,
            selectTranslation: () => of({}),
            translate: (key: string) => key,
            getActiveLang: () => 'en'
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TestTranslationComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
